using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Services
{
    public class LinkService
    {
        private static readonly HashSet<string> ReservedSlugs =
        [
            "api", "admin"
        ];

        private readonly ILinkRepository _linkRepository;
        private readonly IClickRepository _clickRepository;
        private readonly IClickQueue _clickQueue;

        public LinkService(
            ILinkRepository linkRepository,
            IClickRepository clickRepository,
            IClickQueue clickQueue)
        {
            _linkRepository = linkRepository;
            _clickRepository = clickRepository;
            _clickQueue = clickQueue;
        }

        public async Task<CreateLinkResponse> CreateLinkAsync(CreateLinkRequest request, string? baseUrl = null)
        {
            // Validate URL
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Invalid URL format.");
            }

            var slug = request.Slug?.Trim() ?? await GenerateUniqueSlugAsync();

            // Validate slug if provided
            if (string.IsNullOrWhiteSpace(slug) || slug.Length < 3 || slug.Length > 100)
            {
                throw new ArgumentException("Slug must be between 3 and 100 characters.");
            }
            // Check if slug is reserved
            if (ReservedSlugs.Contains(slug.ToLower()))
            {
                throw new ArgumentException("Slug is reserved.");
            }

            if (await DoesSlugExistAsync(slug))
            {
                throw new InvalidOperationException("Slug already exists.");
            }

            var link = new Link
            {
                Id = Guid.NewGuid(),
                Slug = slug,
                Url = request.Url,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpiresAt,
                IsDisabled = false
            };  

            await _linkRepository.AddAsync(link);

            // Use baseUrl if provided, otherwise try to get from environment
            var resolvedBaseUrl = baseUrl ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';').FirstOrDefault() ?? "http://localhost";

            return new CreateLinkResponse
            {
                Slug = link.Slug,
                ShortUrl = $"{resolvedBaseUrl}/{link.Slug}",
                Url = link.Url,
                ExpiresAt = request.ExpiresAt,
                CreatedAt = link.CreatedAt
            };
        }

        public async Task<GetLinkResponse> GetLinkAsync(string slug)
        {
            if (!await DoesSlugExistAsync(slug))
            {
                throw new InvalidOperationException("Slug does not exist.");
            }

            var link = await _linkRepository.GetAsync(slug);

            return new GetLinkResponse
            {
                Slug = link.Slug,
                ShortUrl = $"{Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';').FirstOrDefault() ?? "http://localhost"}/{link.Slug}",
                Url = link.Url,
                ExpiresAt = link.ExpiresAt,
                CreatedAt = link.CreatedAt
            };

        }

        private async Task<string> GenerateUniqueSlugAsync()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            while (true)
            {
                var slug = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                if (!await DoesSlugExistAsync(slug) && !ReservedSlugs.Contains(slug.ToLower()))
                {
                    return slug;
                }
            }
        }

        private async Task<bool> DoesSlugExistAsync(string slug)
        {
            return await _linkRepository.SlugExistsAsync(slug);
        }

        public async Task<ActionResult> DeleteLinkAsync(string slug)
        {
            if (!await DoesSlugExistAsync(slug))
            {
                throw new InvalidOperationException("Slug does not exist.");
            }

            // find link and mark as disabled
            var link = await _linkRepository.GetAsync(slug);
            link.IsDisabled = true;
            await _linkRepository.UpdateAsync(link);

            return new NoContentResult();
        }

        public async Task<Link?> ResolveAsync(
                string slug,
                string? referrer,
                string? userAgent)
        {
            var link = await _linkRepository.GetAsync(slug);

            if (link == null)
            {
                return null;
            }

            if (link.IsDisabled)
            {
                throw new InvalidOperationException("LINK_DISABLED");
            }

            if (link.ExpiresAt.HasValue &&
                link.ExpiresAt.Value <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("LINK_DISABLED");
            }

            // async tracking (non-blocking)
            _clickQueue.Enqueue(new ClickEvent
            {
                LinkId = link.Id,
                Timestamp = DateTime.UtcNow,
                Referrer = referrer,
                UserAgent = userAgent
            });

            return link;
        }
    }
}

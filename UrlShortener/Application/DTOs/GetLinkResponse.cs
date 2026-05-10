namespace UrlShortener.Application.DTOs
{
    public class GetLinkResponse
    {
        public string Slug { get; set; } = default!;
        public string ShortUrl { get; set; } = default!;
        public string Url { get; set; } = default!;
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

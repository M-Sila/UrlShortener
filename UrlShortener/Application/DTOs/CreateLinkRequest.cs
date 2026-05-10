namespace UrlShortener.Application.DTOs
{
    public class CreateLinkRequest
    {
        public string Url { get; set; } = default!;
        public string? Slug { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}

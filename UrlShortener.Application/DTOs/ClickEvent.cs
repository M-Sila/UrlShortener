namespace UrlShortener.Application.DTOs
{
    public class ClickEvent
    {
        public Guid LinkId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Referrer { get; set; }
        public string? UserAgent { get; set; }
    }
}

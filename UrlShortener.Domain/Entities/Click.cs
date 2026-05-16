namespace UrlShortener.Domain.Entities
{
    public class Click
    {
        public Guid Id { get; set; }
        public Guid LinkId { get; set; }
        public Link Link { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string? Referrer { get; set; }
        public string? UserAgent { get; set; }
    }
}

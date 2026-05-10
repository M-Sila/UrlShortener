namespace UrlShortener.Domain.Entities
{
    public class Link
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = default!;
        public string Url { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsDisabled { get; set; } = false;
        public ICollection<Click> Clicks { get; set; } = new List<Click>();
    }
}

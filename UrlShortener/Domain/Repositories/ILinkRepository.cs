using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories
{
    public interface ILinkRepository
    {
        Task AddAsync(Link link);
        Task<Link?> GetAsync(string slug);
        Task<bool> SlugExistsAsync(string slug);
        Task UpdateAsync(Link link);
    }
}

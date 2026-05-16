using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories
{
    public interface IClickRepository
    {
        Task AddAsync(Click click);
    }
}

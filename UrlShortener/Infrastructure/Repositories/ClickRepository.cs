using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class ClickRepository : IClickRepository
    {
        private readonly AppDbContext _dbContext;
        public ClickRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }    
        public async Task AddAsync(Click click)
        {
            _dbContext.Clicks.Add(click);
            await _dbContext.SaveChangesAsync();
        }
    }
}

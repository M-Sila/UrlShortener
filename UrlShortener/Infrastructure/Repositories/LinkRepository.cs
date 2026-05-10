using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly AppDbContext _dbContext;
        public LinkRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }
        public async Task AddAsync(Link link)
        {
            _dbContext.Links.Add(link);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _dbContext.Links.AnyAsync(x => x.Slug == slug);
        }

        public async Task<Link?> GetAsync(string slug)
        {
            return await _dbContext.Links.FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task UpdateAsync(Link link)
        {
            _dbContext.Links.Update(link);
            await _dbContext.SaveChangesAsync();
        }
    }
}

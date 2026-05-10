using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Link> Links => Set<Link>();
        public DbSet<Click> Clicks => Set<Click>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            modelBuilder.Entity<Link>()
                .Property(x => x.Slug)
                .HasMaxLength(32)
                .IsRequired();

            modelBuilder.Entity<Link>()
                .Property(x => x.Url)
                .IsRequired();

            modelBuilder.Entity<Link>()
                .HasMany(x => x.Clicks)
                .WithOne(x => x.Link)
                .HasForeignKey(x => x.LinkId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Click>()
                .HasIndex(x => x.LinkId); 

            modelBuilder.Entity<Click>()
                .HasIndex(x => x.Timestamp); 

            modelBuilder.Entity<Click>()
                .Property(x => x.Referrer)
                .HasMaxLength(2048);

            modelBuilder.Entity<Click>()
                .Property(x => x.UserAgent)
                .HasMaxLength(512);
        }
    }
}

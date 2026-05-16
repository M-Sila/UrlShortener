using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Services
{
    public class ClickBackgroundService : BackgroundService
    {
        private readonly IClickQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public ClickBackgroundService(
            IClickQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var clickEvent = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IClickRepository>();

                    var click = new Click
                    {
                        Id = Guid.NewGuid(),
                        LinkId = clickEvent.LinkId,
                        Timestamp = clickEvent.Timestamp,
                        Referrer = clickEvent.Referrer,
                        UserAgent = clickEvent.UserAgent
                    };

                    await repo.AddAsync(click);
                }
                catch
                {
                    // hier unbedingt logging einbauen (Serilog etc.)
                }
            }
        }
    }
}

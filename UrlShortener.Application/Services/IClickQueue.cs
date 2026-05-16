using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Services
{
    public interface IClickQueue
    {
        void Enqueue(ClickEvent click);
        Task<ClickEvent> DequeueAsync(CancellationToken cancellationToken);
    }
}

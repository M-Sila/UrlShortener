using System.Threading.Channels;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Services
{
    public class ClickQueue : IClickQueue
    {
        private readonly Channel<ClickEvent> _channel;

        public ClickQueue()
        {
            _channel = Channel.CreateUnbounded<ClickEvent>();
        }

        public void Enqueue(ClickEvent click)
        {
            _channel.Writer.TryWrite(click);
        }

        public async Task<ClickEvent> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}

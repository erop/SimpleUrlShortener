using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using SimpleUrlShortener.Core.Contracts;

namespace SimpleUrlShortener.Core.Services;

public class NotificationDispatcher : BackgroundService
{
    private readonly Channel<Notification> _channel;
    private readonly NotificationService _service;

    public NotificationDispatcher(Channel<Notification> channel, NotificationService service)
    {
        _channel = channel;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _channel.Reader.WaitToReadAsync(stoppingToken))
        while (_channel.Reader.TryRead(out var item))
            await _service.SendAsync(item);
    }
}
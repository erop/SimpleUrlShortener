using System.Text.Json;
using Microsoft.Extensions.Options;
using SimpleUrlShortener.Core.Configuration;
using SimpleUrlShortener.Core.Contracts;
using StackExchange.Redis;

namespace SimpleUrlShortener.Core.Services;

public class NotificationService
{
    private readonly RedisSettings _settings;
    private readonly IDatabase _redis;

    public NotificationService(IConnectionMultiplexer multiplexer, IOptions<RedisSettings> settings)
    {
        _redis = multiplexer.GetDatabase();
        _settings = settings.Value;
    }

    public async Task SendAsync(Notification notification)
    {
        await _redis.StreamAddAsync(_settings.Stream, new NameValueEntry[]
        {
            new("id", notification.Timestamp),
            new("payload", JsonSerializer.Serialize(notification))
        });
    }
}
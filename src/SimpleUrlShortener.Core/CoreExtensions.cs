using System.Net;
using System.Text;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleUrlShortener.Core.Configuration;
using SimpleUrlShortener.Core.Contracts;
using SimpleUrlShortener.Core.Persistence;
using SimpleUrlShortener.Core.Services;
using StackExchange.Redis;

namespace SimpleUrlShortener.Core;

public static class CoreExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RedisSettings>()
            .BindConfiguration("RedisSettings")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ShorteningSettings>()
            .BindConfiguration("ShorteningSettings")
            .Validate(settings =>
            {
                if (string.IsNullOrEmpty(settings.Domain)) return false;
                if (settings.DestinationSchemas.Count < 1) return false;
                if (settings.DestinationDomains.Count < 1) return false;
                return true;
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<DataContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            options.UseMySql(connectionString, serverVersion);
        });
        services.AddScoped<IShortener, Shortener>();
        services.AddScoped<IExpander, Expander>();
        services.AddSingleton<PathGenerator>();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            return ConnectionMultiplexer.Connect(settings.Dsn);
        });

        services.AddSingleton<NotificationService>();
        services.AddSingleton<Channel<Notification>>(sp => Channel.CreateUnbounded<Notification>());
        services.AddHostedService<NotificationDispatcher>();

        return services;
    }
}
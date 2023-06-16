using System.Threading.Channels;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleUrlShortener.Core.Configuration;
using SimpleUrlShortener.Core.Contracts;
using SimpleUrlShortener.Core.Entities;
using SimpleUrlShortener.Core.Errors;
using SimpleUrlShortener.Core.Persistence;

namespace SimpleUrlShortener.Core.Services;

public class Shortener : IShortener
{
    private readonly Channel<Notification> _channel;
    private readonly DataContext _context;
    private readonly PathGenerator _generator;
    private readonly ShorteningSettings _settings;

    public Shortener(PathGenerator generator, IOptions<ShorteningSettings> settings, DataContext context,
        Channel<Notification> channel)
    {
        _generator = generator;
        _context = context;
        _channel = channel;
        _settings = settings.Value;
    }

    public async Task<Result<ShorteningResponse>> Shorten(ShorteningRequest request)
    {
        var attempts = 0;
        do
        {
            var path = _generator.Generate();
            var entity = new ShortUrl
            {
                Path = path,
                Destination = request.Destination 
            };

            try
            {
                _context.ShortUrls.Add(entity);
                await _context.SaveChangesAsync();
                var shorteningResponse = new ShorteningResponse(entity.Destination, BuildShortUrl(entity.Path));
                return Result.Ok(shorteningResponse);
            }
            catch (DbUpdateException)
            {
                attempts++;
            }
            catch (OperationCanceledException)
            {
                return Result.Fail(new PathGenerationCancelled());
            }
            catch (Exception ex)
            {
                await _channel.Writer.WriteAsync(new Notification
                {
                    MsgText = ex.Message,
                    MsgCategory = "Application_Alert",
                    MsgType = "UrlShortener error"
                });
                return Result.Fail(new ApplicationError());
            }
        } while (attempts < _settings.MaxAttempts);

        await _channel.Writer.WriteAsync(new Notification
        {
            MsgText = $"The number of short URL generation attempts exceeded {_settings.MaxAttempts}",
            MsgCategory = "Application_Alert",
            MsgType = "UrlShortener error"
        });
        return Result.Fail(new AttemptsExceededError(attempts));
    }

    private Uri BuildShortUrl(string path)
    {
        var builder = new UriBuilder
        {
            Host = _settings.Domain,
            Scheme = "https",
            Path = path
        };
        return builder.Uri;
    }
}
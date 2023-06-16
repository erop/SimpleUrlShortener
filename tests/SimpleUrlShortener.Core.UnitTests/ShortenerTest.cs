using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using SimpleUrlShortener.Core.Configuration;
using SimpleUrlShortener.Core.Contracts;
using SimpleUrlShortener.Core.Entities;
using SimpleUrlShortener.Core.Errors;
using SimpleUrlShortener.Core.Persistence;
using SimpleUrlShortener.Core.Services;

namespace SimpleUrlShortener.Core.UnitTests;

public class ShortenerTest
{
    private readonly Channel<Notification> _channel;
    private readonly Mock<DataContext> _dataContextMock;
    private readonly IOptions<ShorteningSettings> _options;
    private readonly PathGenerator _pathGenerator;

    public ShortenerTest()
    {
        var settings = new ShorteningSettings
        {
            MaxPathLength = 6,
            MaxAttempts = 100,
            AllowedSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
            Domain = "example.com",
            DestinationDomains = new List<string>
            {
                "rbc.ru",
                "habr.com"
            },
            DestinationSchemas = new List<string>
            {
                "https"
            }
        };
        _options = Options.Create(settings);
        _pathGenerator = new PathGenerator(_options);
        _channel = Channel.CreateUnbounded<Notification>();

        _dataContextMock = new Mock<DataContext>();
        IList<ShortUrl> shortUrls = new List<ShortUrl>
        {
            new() {Path = "abc", Destination = new Uri("https://habr.com/first")},
            new() {Path = "def", Destination = new Uri("https://habr.com/second")}
        };

        _dataContextMock.Setup(x => x.ShortUrls).ReturnsDbSet(shortUrls);
    }

    [Fact]
    public async Task Given_AttemptsExceeded_Then_ReceiveFailedResult()
    {
        var pathGeneratorMock = new Mock<PathGenerator>(_options);
        pathGeneratorMock.Setup(p => p.Generate()).Returns("abcdef");

        _dataContextMock.Setup(x => x.SaveChangesAsync(default)).Throws<DbUpdateException>();

        var shortener = new Shortener(pathGeneratorMock.Object, _options, _dataContextMock.Object, _channel);
        var result = await shortener.Shorten(new ShorteningRequest(new Uri("https://example.com")));

        pathGeneratorMock.Verify(p => p.Generate(), Times.Exactly(_options.Value.MaxAttempts));
        Assert.Equal(1, _channel.Reader.Count);
        Assert.True(result.HasError<AttemptsExceededError>());
    }

    [Fact]
    public async Task Given_CancelledOperation_Then_ReceiveFailedResult()
    {
        var pathGeneratorMock = new Mock<PathGenerator>(_options);
        pathGeneratorMock.Setup(p => p.Generate()).Returns("abcdef");

        _dataContextMock.Setup(x => x.SaveChangesAsync(default)).Throws<OperationCanceledException>();

        var shortener = new Shortener(pathGeneratorMock.Object, _options, _dataContextMock.Object, _channel);
        var result = await shortener.Shorten(new ShorteningRequest(new Uri("https://example.com")));

        Assert.Equal(0, _channel.Reader.Count);
        Assert.True(result.HasError<PathGenerationCancelled>());
    }

    [Fact]
    public async Task Given_UnexpectedError_Then_ReceiveFailedResult()
    {
        var pathGeneratorMock = new Mock<PathGenerator>(_options);
        pathGeneratorMock.Setup(p => p.Generate()).Returns("abcdef");

        _dataContextMock.Setup(x => x.SaveChangesAsync(default)).Throws<Exception>();

        var shortener = new Shortener(pathGeneratorMock.Object, _options, _dataContextMock.Object, _channel);
        var result = await shortener.Shorten(new ShorteningRequest(new Uri("https://example.com")));

        Assert.Equal(1, _channel.Reader.Count);
        Assert.True(result.HasError<ApplicationError>());
    }
}
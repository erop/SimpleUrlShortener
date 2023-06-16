using Moq;
using Moq.EntityFrameworkCore;
using SimpleUrlShortener.Core.Contracts;
using SimpleUrlShortener.Core.Entities;
using SimpleUrlShortener.Core.Errors;
using SimpleUrlShortener.Core.Persistence;
using SimpleUrlShortener.Core.Services;

namespace SimpleUrlShortener.Core.UnitTests;

public class ExpanderTest
{
    private readonly Mock<DataContext> _dataContextMock;

    public ExpanderTest()
    {
        _dataContextMock = new Mock<DataContext>();
        IList<ShortUrl> shortUrls = new List<ShortUrl>
        {
            new() {Path = "abc", Destination = new Uri("https://habr.com/first")},
            new() {Path = "def", Destination = new Uri("https://habr.com/second")}
        };

        _dataContextMock.Setup(x => x.ShortUrls).ReturnsDbSet(shortUrls);
    }

    [Fact]
    public async Task Given_ExistingPath_Then_ReturnSuccess()
    {
        var shortener = new Expander(_dataContextMock.Object);
        var result = await shortener.ExpandPath(new ExpandPathRequest("abc"));
        Assert.True(result.IsSuccess);
        Assert.Equal("https://habr.com/first", result.Value.Destination.ToString());
    }

    [Fact]
    public async Task Given_NonExistentPath_Then_ReceiveFailedResult()
    {
        var shortener = new Expander(_dataContextMock.Object);
        var result = await shortener.ExpandPath(new ExpandPathRequest("hjk"));
        Assert.True(result.IsFailed);
        Assert.True(result.HasError<DestinationNotFound>());
    }
}
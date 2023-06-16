using FluentResults;
using SimpleUrlShortener.Core.Contracts;

namespace SimpleUrlShortener.Core.Services;

public interface IShortener
{
    public Task<Result<ShorteningResponse>> Shorten(ShorteningRequest request);
}
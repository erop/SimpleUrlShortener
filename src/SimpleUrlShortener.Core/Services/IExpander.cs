using FluentResults;
using SimpleUrlShortener.Core.Contracts;

namespace SimpleUrlShortener.Core.Services;

public interface IExpander
{
    public Task<Result<ExpandPathResponse>> ExpandPath(ExpandPathRequest request);
}
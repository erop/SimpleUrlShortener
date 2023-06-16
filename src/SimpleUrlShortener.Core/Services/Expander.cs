using FluentResults;
using Microsoft.EntityFrameworkCore;
using SimpleUrlShortener.Core.Contracts;
using SimpleUrlShortener.Core.Errors;
using SimpleUrlShortener.Core.Persistence;

namespace SimpleUrlShortener.Core.Services;

public class Expander : IExpander
{
    private readonly DataContext _context;

    public Expander(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ExpandPathResponse>> ExpandPath(ExpandPathRequest request)
    {
        var shortUrl = await _context.ShortUrls.SingleOrDefaultAsync(x => x.Path == request.Path);
        return shortUrl is null
            ? Result.Fail(new DestinationNotFound(request.Path))
            : Result.Ok(new ExpandPathResponse(shortUrl.Destination));
    }
}
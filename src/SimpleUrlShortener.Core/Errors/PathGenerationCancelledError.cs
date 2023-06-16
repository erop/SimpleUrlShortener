using FluentResults;

namespace SimpleUrlShortener.Core.Errors;

public class PathGenerationCancelled : Error
{
    public PathGenerationCancelled(): base("Path generation was cancelled")
    {
    }
}
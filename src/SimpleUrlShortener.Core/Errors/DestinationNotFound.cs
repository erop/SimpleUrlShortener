using FluentResults;

namespace SimpleUrlShortener.Core.Errors;

public class DestinationNotFound : Error
{
    public DestinationNotFound(string path) : base($"Could not find destination URL for the path provided: '{path}'")
    {
    }
}
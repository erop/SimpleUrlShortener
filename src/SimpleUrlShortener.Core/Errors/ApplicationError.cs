using FluentResults;

namespace SimpleUrlShortener.Core.Errors;

public class ApplicationError : Error
{
    public ApplicationError() : base("An application error occurred")
    {
    }
}
using FluentResults;

namespace SimpleUrlShortener.Core.Errors;

public class AttemptsExceededError : Error
{
    public AttemptsExceededError(int attemptsCount) : base(
        $"The number of generation attempts ({attemptsCount}) exceeded")
    {
    }
}
using SimpleUrlShortener.Core.Validation;

namespace SimpleUrlShortener.Core.Contracts;

public record ShorteningRequest([AllowedDestination] Uri Destination);
namespace SimpleUrlShortener.Core.Contracts;

public record ShorteningResponse(Uri Destination, Uri Short);
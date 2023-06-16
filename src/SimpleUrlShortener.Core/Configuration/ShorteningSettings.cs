using System.ComponentModel.DataAnnotations;

namespace SimpleUrlShortener.Core.Configuration;

public class ShorteningSettings
{
    [Required] public int MaxPathLength { get; set; }

    [Required] public int MaxAttempts { get; set; }

    [Required] public string AllowedSymbols { get; set; } = null!;

    [Required] public string Domain { get; set; } = null!;

    public List<string> DestinationDomains { get; set; } = new();
    public List<string> DestinationSchemas { get; set; } = new();
}
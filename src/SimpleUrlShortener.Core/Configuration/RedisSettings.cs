using System.ComponentModel.DataAnnotations;

namespace SimpleUrlShortener.Core.Configuration;

public class RedisSettings
{
    [Required] public string Dsn { get; set; } = null!;
    [Required] public string Stream { get; set; } = null!;
}
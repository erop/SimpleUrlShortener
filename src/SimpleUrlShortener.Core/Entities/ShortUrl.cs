using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleUrlShortener.Core.Entities;

public class ShortUrl
{
    [Key]
    [Column(TypeName = "varchar(10)")]
    public required string Path { get; init; }

    public required Uri Destination { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;
}
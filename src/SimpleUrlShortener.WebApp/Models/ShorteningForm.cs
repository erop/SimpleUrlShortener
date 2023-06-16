using System.ComponentModel.DataAnnotations;
using SimpleUrlShortener.Core.Validation;

namespace SimpleUrlShortener.WebApp.Models;

public class ShorteningForm
{
    [AllowedDestination]
    public Uri Destination { get; set; } = null!;
}
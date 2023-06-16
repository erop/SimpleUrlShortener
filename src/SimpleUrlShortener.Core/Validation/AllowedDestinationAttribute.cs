using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleUrlShortener.Core.Configuration;

namespace SimpleUrlShortener.Core.Validation;

public class AllowedDestinationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var options = validationContext.GetService<IOptions<ShorteningSettings>>();
        if (options is null) return new ValidationResult("Unable to acquire allowed domains");
        var settings = options.Value;

        if (value is null) return new ValidationResult("No URL provided");
        var destination = (Uri) value;

        if (!destination.IsAbsoluteUri) return new ValidationResult("Only absolute URL allowed");

        if (!settings.DestinationSchemas.Contains(destination.Scheme))
        {
            var template = "Only one of the schemas '{0}' allowed";
            if (settings.DestinationSchemas.Count == 1) template = "Only '{0}' schema allowed";

            return new ValidationResult(string.Format(template, string.Join(", ", settings.DestinationSchemas)));
        }

        if (!settings.DestinationDomains.Contains(destination.Host))
        {
            return new ValidationResult("Invalid domain in URL");
        }

        return ValidationResult.Success;
    }
}
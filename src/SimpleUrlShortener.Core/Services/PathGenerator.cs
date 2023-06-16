using System.Text;
using Microsoft.Extensions.Options;
using SimpleUrlShortener.Core.Configuration;

namespace SimpleUrlShortener.Core.Services;

public class PathGenerator
{
    private readonly ShorteningSettings _settings;

    public PathGenerator(IOptions<ShorteningSettings> options)
    {
        _settings = options.Value;
    }

    public virtual string Generate()
    {
        var randomMaxValue = _settings.AllowedSymbols.Length - 1;
        var chars = _settings.AllowedSymbols.ToCharArray();
        var stringBuilder = new StringBuilder();
        var random = new Random();
        while (stringBuilder.Length < _settings.MaxPathLength)
        {
            var ch = chars[random.Next(0, randomMaxValue)];
            stringBuilder.Append(ch);
        }

        return stringBuilder.ToString();
    }
}
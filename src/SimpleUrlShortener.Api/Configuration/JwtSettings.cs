namespace SimpleUrlShortener.Api.Configuration;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public string CookieName { get; set; } = string.Empty;
}
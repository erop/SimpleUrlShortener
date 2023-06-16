using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleUrlShortener.Api.Configuration;

namespace SimpleUrlShortener.Api.Authentication;

public class TokenAuthenticationSchemeHandler : AuthenticationHandler<TokenAuthenticationSchemeOptions>
{
    private readonly JwtSettings _jwtSettings;

    public TokenAuthenticationSchemeHandler(IOptionsMonitor<TokenAuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IOptionsMonitor<JwtSettings> jwtSettings) : base(
        options, logger, encoder, clock)
    {
        _jwtSettings = jwtSettings.CurrentValue;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.ContainsKey(_jwtSettings.CookieName))
            return Task.FromResult(
                AuthenticateResult.Fail($"No authentication cookie provided: {_jwtSettings.CookieName}"));

        var jwt = Request.Cookies[_jwtSettings.CookieName];

        var jsonWebTokenHandler = new JsonWebTokenHandler();
        var tokenValidationResult = jsonWebTokenHandler.ValidateToken(jwt, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidIssuer = _jwtSettings.Issuer
        });

        if (!tokenValidationResult.IsValid)
            return Task.FromResult(tokenValidationResult.Exception is not null
                ? AuthenticateResult.Fail(tokenValidationResult.Exception.Message)
                : AuthenticateResult.Fail("Token validation failed"));

        var userId = (string) tokenValidationResult.Claims["sub"];

        var claims = new List<Claim> {new("userId", userId)};
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(TokenAuthenticationSchemeHandler)));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
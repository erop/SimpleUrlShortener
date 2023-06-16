using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using SimpleUrlShortener.Api;
using SimpleUrlShortener.Api.Authentication;
using SimpleUrlShortener.Api.Configuration;
using SimpleUrlShortener.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection(Constants.AllowedOrigins).Get<string[]>();
        if (allowedOrigins is null) throw new Exception("No allowed origins provided");
        policyBuilder.WithOrigins(allowedOrigins);
    });
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .Validate(settings => true)
    .ValidateOnStart();

builder.Services.AddCoreServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Nextologies URL Shortener API",
        Description = "Use our service to shorten your URLs!"
    });
    options.AddSecurityDefinition("UserNotificationToken", new OpenApiSecurityScheme
    {
        // Type = SecuritySchemeType.Http,
        In = ParameterLocation.Cookie,
        Name = "USER_NOTIFICATION_TOKEN",
        Description = "Authentication with JWT for Notification service"
    });
});

builder.Services.AddAuthentication(Constants.UserNotificationToken)
    .AddScheme<TokenAuthenticationSchemeOptions, TokenAuthenticationSchemeHandler>(Constants.UserNotificationToken,
        options => { });


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
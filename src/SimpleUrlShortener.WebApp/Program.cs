using Microsoft.AspNetCore.HttpOverrides;
using SimpleUrlShortener.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();
app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();

app.MapRazorPages();

app.Run();
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleUrlShortener.Core.Contracts;
using SimpleUrlShortener.Core.Services;
using SimpleUrlShortener.Core.Validation;

namespace SimpleUrlShortener.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IExpander _expander;
    private readonly IShortener _shortener;

    public IndexModel(ILogger<IndexModel> logger, IShortener shortener, IExpander expander)
    {
        _shortener = shortener;
        _expander = expander;
    }

    [BindProperty(SupportsGet = true)] public string? Path { get; set; }

    [BindProperty] [AllowedDestination] public required Uri Destination { get; set; }

    public Uri? ShortUrl { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        try
        {
            if (string.IsNullOrEmpty(Path)) return Page();

            var result = await _expander.ExpandPath(new ExpandPathRequest(Path));

            if (result.IsFailed) return NotFound();

            return Redirect(result.Value.Destination.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _shortener.Shorten(new ShorteningRequest(Destination));

        if (result.IsFailed)
        {
            var error = result.Errors[0];
            ErrorMessage = error.Message;
        }
        else
        {
            ShortUrl = result.Value.Short;
        }

        return Page();
    }
}
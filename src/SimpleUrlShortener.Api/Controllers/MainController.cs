using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleUrlShortener.Core.Contracts;
using SimpleUrlShortener.Core.Errors;
using SimpleUrlShortener.Core.Services;

namespace SimpleUrlShortener.Api.Controllers;

[ApiController]
public class MainController : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShorteningResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("/api/generate")]
    public async Task<ActionResult<ShorteningResponse>> Generate(ShorteningRequest request,
        [FromServices] IShortener shortener)
    {
        var result = await shortener.Shorten(request);
        if (result.IsFailed)
        {
            var error = result.Errors[0];
            if (error is ApplicationError)
                return Problem(statusCode: StatusCodes.Status503ServiceUnavailable, detail: error.Message,
                    title: error.Message);
            if (error is AttemptsExceededError)
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Message,
                    title: error.Message);
        }

        return Ok(result.Value);
    }
}
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services.Abstraction;

namespace UrlShortener.Controllers;

[ApiController]
[Route("")]
public class RedirectController(IShortUrlService _shortUrlService) : ControllerBase
{
    [HttpGet("{url}")]
    public async Task<IActionResult> ResolveUrl(string url)
    {
        var result = await _shortUrlService.ResolveAsync(url);

        return result.IsSuccess
            ? RedirectPermanent(result.Data.LongUrl)
            : NotFound(result.Errors);
    }
}
using Microsoft.AspNetCore.Mvc;
using UrlShortener.DTOs;
using UrlShortener.Services.Abstraction;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlController(IShortUrlService _shortUrlService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateShortUrl(CreateShortUrlDto url)
        {
            var result  = await _shortUrlService.AddShortUrlAsync(url.LongUrl);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{url}")]
        public async Task<IActionResult> ResolveUrl([FromRoute]string url)
        {
            var result  = await _shortUrlService.ResolveAsync(url);

            return result.IsSuccess ? RedirectPermanent(result.Data.LongUrl) : BadRequest(result.Errors);
        }
    }
}
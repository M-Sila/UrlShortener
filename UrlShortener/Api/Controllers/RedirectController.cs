using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Services;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly LinkService _linkService;

        public RedirectController(LinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> RedirectToOriginalUrl(string slug)
        {
            try
            {
                var link = await _linkService.ResolveAsync(
                    slug,
                    Request.Headers.Referer.ToString(),
                    Request.Headers.UserAgent.ToString());

                if (link == null)
                {
                    return NotFound(new
                    {
                        Message = "Short link not found."
                    });
                }

                return Redirect(link.Url);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "LINK_DISABLED")
                {
                    return BadRequest(new
                    {
                        Message = "Link is disabled or expired."
                    });
                }

                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
    }
    }

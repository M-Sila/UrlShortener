using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Services;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinksController : ControllerBase
    {
        private readonly LinkService _linkService;

        public LinksController(LinkService linkService)
        {
            _linkService = linkService;
        }
        [HttpPost]
        public async Task<ActionResult> CreateLink()
        {
            return Ok();
        }
    }
}

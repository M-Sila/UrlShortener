using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs;
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
        public async Task<CreateLinkResponse> CreateLink(CreateLinkRequest request)
        {
            var result = await _linkService.CreateLinkAsync(request);
            return result;
        }

        [HttpGet("{slug}")]
        public async Task<GetLinkResponse> GetLink(string slug)
        {
            var result = await _linkService.GetLinkAsync(slug);
            return result;
        }

        [HttpDelete("{slug}")]
        public async Task<bool> DeleteLink(string slug)
        {
            var result = await _linkService.DeleteLinkAsync(slug);
            return result;
        }
    }
}

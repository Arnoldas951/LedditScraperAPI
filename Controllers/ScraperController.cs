using LedditScraperAPI.Scraper.Handlers;
using LedditScraperAPI.Scraper.Handlers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace LedditScraperAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IGetLedditJsonService _ledditJsonService;

        public ScraperController(IGetLedditJsonService ledditJsonService)
        {
            _ledditJsonService = ledditJsonService;
        }

        [HttpGet]
        public IActionResult Get(string subreddit)
        {
            var result = _ledditJsonService.GetLedditData(subreddit);

            return new JsonResult(result);
        }
    }
}

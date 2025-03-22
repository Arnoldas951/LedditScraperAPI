using LedditScraperAPI.Scraper.Handlers;
using LedditScraperAPI.Scraper.Handlers.Abstractions;
using LedditScraperAPI.Scraper.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using System.Net.Mail;

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

        [HttpPost("DownloadVideos")]

        public async Task<IActionResult> DownloadVidoes([FromBody] string[] downloadLinks)
        {
            if (downloadLinks.Any())
            {
                try
                {
                    var memoryStream = new MemoryStream();
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var req in downloadLinks)
                        {
                            var result = await _ledditJsonService.DownloadVideo(req);
                            var entry = zipArchive.CreateEntry(DateTime.Now.ToShortTimeString()+".mp4");
                            using (var entryStream = entry.Open())
                            {
                                await entryStream.WriteAsync(result, 0, result.Length);
                            }
                        }
                    }

                    return File(memoryStream.ToArray(), "application/zip", $"download_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip");

                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");
                }
            }

            return BadRequest("no links");
        }
    }
}

using LedditScraperAPI.Scraper.Models;

namespace LedditScraperAPI.Scraper.Handlers.Abstractions
{
    public interface IGetLedditJsonService
    {
        Task<IEnumerable<LedditJsonModel>> GetLedditData(string subreddit);
        Task<Byte[]> DownloadVideo(string downloadLink);
    }
}

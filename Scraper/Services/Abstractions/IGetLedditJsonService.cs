using LedditScraperAPI.Scraper.Models;

namespace LedditScraperAPI.Scraper.Handlers.Abstractions
{
    public interface IGetLedditJsonService
    {
        IEnumerable<LedditJsonModel> GetLedditData(string subreddit);
    }
}

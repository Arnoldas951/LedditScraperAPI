namespace LedditScraperAPI.Scraper.Models
{
    public class LedditJsonModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }

        public int? MediaLength { get; set; }

        public string? DownloadLink { get; set; }

        public string? UpvoteRatio { get; set; }

        public bool IsNsfw { get; set; }
    }
}

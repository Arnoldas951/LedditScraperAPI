using LedditScraperAPI.Scraper.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using LedditScraper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LedditScraperAPI.Scraper.Handlers.Abstractions;

namespace LedditScraperAPI.Scraper.Handlers
{

    public class GetLedditJsonService : IGetLedditJsonService
    {
        public IEnumerable<LedditJsonModel> GetLedditData(string subreddit)
        {
            var objectList = new List<LedditJsonModel>();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            // Used to workaround headless driver block
            chromeOptions.AddArguments("user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
            IWebDriver driver;
            driver = new ChromeDriver(chromeOptions);
            driver.Url = StaticLinks.RedditPrefix + subreddit + StaticLinks.RedditPostFix;

            var jsonContent = driver.FindElement(By.TagName("pre")).Text;
            driver.Close();

            var root = (JObject)JsonConvert.DeserializeObject(jsonContent);
            var items = root.SelectToken("data").Children();
            var childItems = items.Children().ToList();
            var childerItems = childItems.Children().ToList();
            foreach (var item in childerItems)
            {
                if (!string.IsNullOrWhiteSpace(item["data"]["secure_media"].ToString()))
                {
                    var newModel = new LedditJsonModel();
                    newModel.Id = Guid.NewGuid();
                    newModel.MediaLength = int.Parse(item["data"]["secure_media"]["reddit_video"]["duration"].ToString());
                    newModel.DownloadLink = StaticLinks.RedditPrefix + item["data"]["permalink"];
                    newModel.Title = item["data"]["title"].ToString();
                    newModel.UpvoteRatio = item["data"]["upvote_ratio"].ToString();
                    newModel.IsNsfw = item["data"]["nsfw"] == null ? false : true;
                    objectList.Add(newModel);
                }
            }
            return objectList;
        }
    }
}

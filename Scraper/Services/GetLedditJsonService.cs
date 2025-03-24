using LedditScraperAPI.Scraper.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using LedditScraper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LedditScraperAPI.Scraper.Handlers.Abstractions;
using System;
using LedditScraperAPI.Extensions;
using Microsoft.Extensions.Options;

namespace LedditScraperAPI.Scraper.Handlers
{

    public class GetLedditJsonService : IGetLedditJsonService
    {
        public IEnumerable<LedditJsonModel> GetLedditData(string subreddit)
        {
            //var service = ChromeDriverService.CreateDefaultService();
            //service.LogPath = "/tmp/chromedriver.log";
            //service.EnableVerboseLogging = true;

            var objectList = new List<LedditJsonModel>();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            //chromeOptions.BinaryLocation = "/usr/bin/google-chrome-stable";
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
                    newModel.DownloadLink = StaticLinks.Reddit + item["data"]["permalink"];
                    newModel.Title = item["data"]["title"].ToString();
                    newModel.UpvoteRatio = item["data"]["upvote_ratio"].ToString();
                    newModel.IsNsfw = item["data"]["nsfw"] == null ? false : true;
                    objectList.Add(newModel);
                }
            }
            return objectList;
        }

        public async Task<byte[]> DownloadVideo(string downloadLink)
        {
            byte[] fileContent = [];
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");

            ChromeDriver driver;
            driver = new ChromeDriver(options);
            driver.Url = StaticLinks.DownloadLink + downloadLink.ChangeSlashes();
            var reportDownloadButton = driver.FindElement(By.XPath("//a[normalize-space() = 'Download HD Video']"));
            string downloadUrl = reportDownloadButton.GetAttribute("href");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");

                // Download the content
                fileContent = await httpClient.GetByteArrayAsync(downloadUrl);

            }

            return fileContent;
        }
    }
}

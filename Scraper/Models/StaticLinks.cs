using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedditScraper.Models
{
    public static class StaticLinks
    {
        public static string DownloadLink => "https://rapidsave.com/info?url=";
        public static string RedditPrefix => "https://www.reddit.com/r/";

        public static string RedditPostFix => "/hot/.json?limit=5";
        //public static string KidsRStupidHot => "https://www.reddit.com/r/Kidsarefuckingstupid/hot/.json?limit=5";
    }
}

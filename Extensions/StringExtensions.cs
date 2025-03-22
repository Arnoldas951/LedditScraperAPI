using System.Text;

namespace LedditScraperAPI.Extensions
{
    public static class StringExtensions
    {
        public static string ChangeSlashes(this string str)
        {
            StringBuilder sb = new StringBuilder(str);

            sb.Replace("/", "%2F");
            sb.Replace(":", "%3A");
            return sb.ToString();
        }
    }
}

using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace SerpScraper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string searching = "";
            string current_Search = "Steam ";
            string steamCardexchangeURL = "https://www.steamcardexchange.net/index.php?gamepage-appid-";
            string GamesUrl;
            string GameCode;
            bool isFoilCards = false;

            Console.WriteLine("Enter the game u wana search for its STEAM Trading cards");
            searching += Console.ReadLine();
            current_Search += searching;

            Console.WriteLine("ur search is: " + current_Search);

            var results = ScrapeSerp(current_Search, 1, 1);//search string, n pages, x results
            foreach (var result in results)
            {
                Console.WriteLine(result.Title); //link Title
                Console.WriteLine("https://steamcommunity.com/market/search?q=" + searching); //steam market link for the game ur searching for
                Console.WriteLine(result.Url); //game url in steam
                GamesUrl = result.Url;
                GameCode = Regex.Match(GamesUrl, @"\d+").Value; //seperate the game code from the game url
                steamCardexchangeURL += GameCode;
                Console.WriteLine(steamCardexchangeURL); //steam codex for steam games trading cards
            }
            var SteamTradingCardsLinksResults = ScrapeSerpForSteamCardex(steamCardexchangeURL);
            foreach (var SteamTradingCardLinksResult in SteamTradingCardsLinksResults)
            {
                if (!isFoilCards)
                {
                    if (StringExtension.GetLast(SteamTradingCardLinksResult.Url, 6) == "(Foil)")
                    {
                        break;
                    }
                }
                else
                {
                    if (StringExtension.GetLast(SteamTradingCardLinksResult.Url, 17) == "%20Booster%20Pack")
                    {
                        break;
                    }
                }
                Console.WriteLine(SteamTradingCardLinksResult.Url);
                var PriceSteamTradingCards = ScrapSerpForTradingCardPrice(SteamTradingCardLinksResult.Url);
                foreach (var Price in PriceSteamTradingCards)
                {
                    Console.WriteLine("...");
                    break;//just for now
                }
                break;//just for now
            }
        }
        public static List<serpResult> ScrapeSerp(string query, int n_pages, int x_results)
        {
            var serpResults = new List<serpResult>();
            for (var i = 1; i <= n_pages; i++)
            {
                var url = "http://www.google.com/search?q=" + query + $" &num={x_results}&start=" + ((i - 1) * 10).ToString();
                HtmlWeb web = new HtmlWeb();
                web.UserAgent = "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";

                var htmldoc = web.Load(url);
                HtmlNodeCollection Nodes = htmldoc.DocumentNode.SelectNodes("//div[@class='yuRUbf']");
                foreach (var tag in Nodes)
                {
                    var result = new serpResult();
                    result.Url = tag.Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    result.Title = tag.Descendants("h3").FirstOrDefault().InnerText;
                    serpResults.Add(result);
                }
            }
            return serpResults;
        }
        public static List<serpResult> ScrapeSerpForSteamCardex(string steamCardexchangeURL)
        {
            var ScrapeSerpForSteamCardex = new List<serpResult>();
            for (var i = 1; i <= 1; i++)
            {
                var url = steamCardexchangeURL;

                HtmlWeb web = new HtmlWeb();
                web.UserAgent = "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";

                var htmldoc = web.Load(url);
                HtmlNodeCollection Nodes = htmldoc.DocumentNode.SelectNodes("//div[@class='flex flex-col items-center p-5 gap-y-2 bg-gray-light']");
                foreach (var tag in Nodes)
                {
                    var result = new serpResult();
                    result.Url = tag.Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    ScrapeSerpForSteamCardex.Add(result);
                }
                break;
            }
            return ScrapeSerpForSteamCardex;
        }
        public static List<serpResult> ScrapSerpForTradingCardPrice(string steamTradingCardUrl)
        {
            Console.WriteLine("yes");
            string cardCodeForPreference;
            string PriceUrlForCurrentCardUrl = "https://steamcommunity.com/market/itemordershistogram?country=US&language=english&currency=34&item_nameid=";
            var ScrapSerpForTradingCardPrice = new List<serpResult>();
            for (var i = 1; i <= 1; i++)
            {
                var url = steamTradingCardUrl;
                HtmlWeb web = new HtmlWeb();
                web.UserAgent = "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";

                var htmldoc = web.Load(url);
                HtmlNodeCollection Nodes = htmldoc.DocumentNode.SelectNodes("//script[@type='text/javascript']");
                //Console.WriteLine(Nodes.Count);

                cardCodeForPreference = Regex.Match(StringExtension.GetLast(Nodes[Nodes.Count - 1].InnerText, 35), @"\d+").Value;
                PriceUrlForCurrentCardUrl += cardCodeForPreference + "&two_factor=0";

                var htmldoc2 = web.Load(PriceUrlForCurrentCardUrl);
                Console.WriteLine("___> ", htmldoc2.Text);

                //HtmlNodeCollection Nodes2 = htmldoc2.DocumentNode.SelectNodes("//pre");
                //Console.WriteLine(Nodes2.Count);

                //foreach (var tag in Nodes)
                //{
                //    var result = new serpResult();
                //    Console.WriteLine("----> ");
                //    result.Price = "yes";
                //    ScrapSerpForTradingCardPrice.Add(result);
                //}
            }
            return ScrapSerpForTradingCardPrice;
        }
        public class serpResult
        {
            public string Url { get; set; }
            public string Title { get; set; }
            public string Price { get; set; }
        }
    }
    public static class StringExtension
    {
        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
    }
}
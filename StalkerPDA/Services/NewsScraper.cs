using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StalkerPDA.Models;

namespace StalkerPDA.Services
{
    public class NewsScraper
    {
        private readonly HttpClient _httpClient;

        public NewsScraper()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<List<NewsItem>> FetchLatestNewsAsync()
        {
            var newsList = new List<NewsItem>();

            try
            {
                string url = "https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=1643320&count=10&maxlength=0&format=json";

                string json = await _httpClient.GetStringAsync(url);
                var root = JObject.Parse(json);
                var newsItems = root["appnews"]["newsitems"];

                foreach (var item in newsItems)
                {
                    if (item["feedname"]?.ToString() == "steam_community_announcements")
                    {
                        string title = item["title"]?.ToString();
                        string contents = item["contents"]?.ToString();
                        string articleUrl = item["url"]?.ToString();

                        long timestamp = (long)item["date"];
                        DateTime date = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                        string dateString = date.ToString("dd.MM.yyyy");

                        string cleanText = CleanText(contents);

                        newsList.Add(new NewsItem(title, cleanText, dateString, articleUrl, false));

                        if (newsList.Count >= 5) break;
                    }
                }

                if (newsList.Count == 0)
                {
                    newsList.Add(new NewsItem("Мережа мовчить", "Нових повідомлень немає.", DateTime.Now.ToString("dd.MM.yyyy"), "", false));
                }
            }
            catch
            {
                newsList.Add(new NewsItem("Втрата зв'язку", "Мережа Зони недоступна. Помилка підключення.", DateTime.Now.ToString("dd.MM.yyyy"), "", true));
            }

            return newsList;
        }

        private string CleanText(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            string text = input.Replace("[br]", "\n").Replace("<br>", "\n");

            text = Regex.Replace(text, @"\[.*?\]", "");
            text = Regex.Replace(text, @"<.*?>", "");

            return text.Trim();
        }
    }
}
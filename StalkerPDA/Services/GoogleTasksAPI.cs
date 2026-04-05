using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StalkerPDA.Services
{
    public class GoogleTasksAPI
    {
        private const string ClientId = "КЛЮЧ";
        private const string ClientSecret = "КЛЮЧ";
        private const string RefreshToken = "КЛЮЧ";

        public async Task<List<string>> GetActiveQuestsAsync()
        {
            var quests = new List<string>();

            try
            {
                using var client = new HttpClient();
                var dict = new Dictionary<string, string>
                {
                    { "client_id", ClientId },
                    { "client_secret", ClientSecret },
                    { "refresh_token", RefreshToken },
                    { "grant_type", "refresh_token" }
                };

                var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(dict));

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    string errorText = await tokenResponse.Content.ReadAsStringAsync();
                    quests.Add("ПОМИЛКА: ПДА не може підключитися до супутника.");
                    quests.Add($"Код від Google: {errorText}");
                    return quests;
                }

                var tokenJson = JObject.Parse(await tokenResponse.Content.ReadAsStringAsync());
                var token = tokenJson["access_token"]?.ToString();

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var tasksResponse = await client.GetStringAsync("https://tasks.googleapis.com/tasks/v1/lists/@default/tasks?showCompleted=false");
                var items = JObject.Parse(tasksResponse)["items"];

                if (items != null && items.HasValues)
                {
                    foreach (var item in items)
                    {
                        quests.Add(item["title"].ToString());
                    }
                }
                else
                {
                    quests.Add("Немає активних квестів. Можна відпочити біля вогнища.");
                }
            }
            catch (Exception ex)
            {
                quests.Add("КРИТИЧНА ПОМИЛКА: Зв'язок з базою даних втрачено.");
                quests.Add(ex.Message);
            }

            return quests;
        }
    }
}
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Android.Util;

namespace StalkerPDA.Services
{
    public class GeminiService
    {
        private const string GeminiKey = "КЛЮЧ";
        private const string GroqKey = "КЛЮЧ";   

        private const string GroqModel = "llama-3.1-8b-instant";

        public async Task<string> SendConsciousnessMessageAsync(string userMessage)
        {
            string sysPrompt = "Ти — інтелектуальний термінал системи 'О-Свідомість'. " +
                               "Твій стиль: суворий, науково-містичний, лаконічний. " +
                               "КРИТИЧНО ВАЖЛИВО: Використовуй тільки існуючі слова української мови. " +
                               "Дотримуйся правил граматики та пунктуації. Не вигадуй нових термінів. " +
                               "Ти — колективний розум Зони, говори чітко, без 'словесного сміття'.";

            string offlineResponse = "[СИСТЕМА]: Критична помилка зв'язку з Ноосферою. Спробуйте пізніше.";

            return await ExecuteTripleFallbackAsync(sysPrompt, userMessage, offlineResponse);
        }

        private async Task<string> ExecuteTripleFallbackAsync(string systemContent, string userContent, string offlineFallback)
        {
            string timeTag = DateTime.Now.ToString("HH:mm:ss");

            string combinedPrompt = $"ІНСТРУКЦІЯ: {systemContent}\n\nКОРИСТУВАЧ: {userContent}";

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(7) };

                var req = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[] { new { text = combinedPrompt } }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.4,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 400
                    }
                };

                var res = await client.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={GeminiKey}",
                    new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json")
                );

                string raw = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode)
                {
                    var j = JObject.Parse(raw);
                    string result = j["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Log.Debug("PDA_LOG", $"[{timeTag}] Gemini OK");
                        return result.Trim();
                    }
                }
                Log.Debug("PDA_LOG", $"[{timeTag}] Gemini FAIL → fallback to Groq");
            }
            catch (Exception ex)
            {
                Log.Error("PDA_LOG", $"[{timeTag}] Gemini ERROR: {ex.Message}");
            }

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
                request.Headers.Add("Authorization", $"Bearer {GroqKey}");

                var body = new
                {
                    model = GroqModel,
                    messages = new[]
                    {
                        new { role = "system", content = systemContent },
                        new { role = "user", content = userContent }
                    },
                    temperature = 0.5 
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var res = await client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    var resText = await res.Content.ReadAsStringAsync();
                    var j = JObject.Parse(resText);
                    string result = j["choices"]?[0]?["message"]?["content"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Log.Debug("PDA_LOG", $"[{timeTag}] Groq OK");
                        return result.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("PDA_LOG", $"[{timeTag}] Groq ERROR: {ex.Message}");
            }

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                string url = $"https://text.pollinations.ai/{Uri.EscapeDataString(combinedPrompt)}";
                var result = await client.GetStringAsync(url);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    Log.Debug("PDA_LOG", $"[{timeTag}] Pollinations OK");
                    return result.Trim();
                }
            }
            catch (Exception ex)
            {
                Log.Error("PDA_LOG", $"[{timeTag}] Pollinations ERROR: {ex.Message}");
            }

            return offlineFallback;
        }
    }
}
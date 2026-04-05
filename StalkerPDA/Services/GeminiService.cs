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
            string sysPrompt = "Ти — 'О-Свідомість'. Відповідай українською, загадково. Ти — розум Зони.";
            string offlineResponse = "[СИСТЕМА]: Ноосфера закрита викидом.";

            return await ExecuteTripleFallbackAsync(sysPrompt, userMessage, offlineResponse);
        }

        private async Task<string> ExecuteTripleFallbackAsync(string systemContent, string userContent, string offlineFallback)
        {
            string timeTag = DateTime.Now.ToString("HH:mm:ss");
            string combinedPrompt = $"{systemContent}\nЗапит: {userContent}";

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(7) };

                var req = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = combinedPrompt }
                            }
                        }
                    }
                };

                var res = await client.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={GeminiKey}",
                    new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json")
                );

                string raw = await res.Content.ReadAsStringAsync();
                Log.Debug("PDA_LOG", $"[{timeTag}] Gemini RAW: {raw}");

                if (res.IsSuccessStatusCode)
                {
                    var j = JObject.Parse(raw);

                    string result = j["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Log.Debug("PDA_LOG", $"[{timeTag}] Gemini RESPONSE: {result}");
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

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.groq.com/openai/v1/chat/completions"
                );

                request.Headers.Add("Authorization", $"Bearer {GroqKey}");

                var body = new
                {
                    model = GroqModel,
                    messages = new[]
                    {
                        new { role = "system", content = systemContent },
                        new { role = "user", content = userContent }
                    },
                    temperature = 0.7
                };

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(body),
                    Encoding.UTF8,
                    "application/json"
                );

                Log.Debug("PDA_LOG", $"[{timeTag}] Groq: Sending...");

                var res = await client.SendAsync(request);
                string resText = await res.Content.ReadAsStringAsync();

                Log.Debug("PDA_LOG", $"[{timeTag}] Groq RAW: {resText}");
                Log.Debug("PDA_LOG", $"[{timeTag}] Groq: Status {res.StatusCode}");

                if (res.IsSuccessStatusCode)
                {
                    var j = JObject.Parse(resText);

                    string result =
                        j["choices"]?[0]?["message"]?["content"]?.ToString()
                        ?? j["choices"]?[0]?["delta"]?["content"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Log.Debug("PDA_LOG", $"[{timeTag}] Groq RESPONSE: {result}");
                        return result.Trim();
                    }
                }

                Log.Error("PDA_LOG", $"[{timeTag}] Groq FAIL (empty or bad response)");
            }
            catch (Exception ex)
            {
                Log.Error("PDA_LOG", $"[{timeTag}] Groq ERROR: {ex.Message}");
            }

            try
            {
                Log.Debug("PDA_LOG", $"[{timeTag}] Pollinations fallback...");

                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

                string url = $"https://text.pollinations.ai/{Uri.EscapeDataString(combinedPrompt)}";

                var result = await client.GetStringAsync(url);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    Log.Debug("PDA_LOG", $"[{timeTag}] Pollinations RESPONSE: {result}");
                    return result.Trim();
                }
            }
            catch (Exception ex)
            {
                Log.Error("PDA_LOG", $"[{timeTag}] Pollinations ERROR: {ex.Message}");
            }

            Log.Debug("PDA_LOG", $"[{timeTag}] OFFLINE FALLBACK");
            return offlineFallback;
        }
    }
}
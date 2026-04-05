using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StalkerPDA.Services
{
    public class WeatherAPI
    {
        public async Task<string> GetWeatherAsync()
        {
            try
            {
                using var client = new HttpClient();
                string url = "https://api.open-meteo.com/v1/forecast?latitude=50.45&longitude=30.52&current_weather=true";
                string response = await client.GetStringAsync(url);
                var data = JObject.Parse(response);
                var temp = data["current_weather"]?["temperature"]?.ToString();
                return $"Погода: {temp}°C. Датчики в норме.";
            }
            catch
            {
                return "Погода: Нет связи с метеостанцией.";
            }
        }
    }
}
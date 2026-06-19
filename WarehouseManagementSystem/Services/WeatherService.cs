using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WarehouseManagementSystem.Services
{
    public class WeatherForecast
    {
        public string Region { get; set; }
        public DateTime Date { get; set; }
        public int Temperature { get; set; }
        public string Condition { get; set; }
        public bool IsAnomaly { get; set; }
        public string Recommendation { get; set; }
    }

    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, (double lat, double lon, int avgTemp)> _regionData;

        public WeatherService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            _regionData = new Dictionary<string, (double, double, int)>
            {
                { "Москва", (55.75, 37.62, 18) },
                { "Санкт-Петербург", (59.93, 30.31, 15) },
                { "Новосибирск", (55.03, 82.92, 14) },
                { "Екатеринбург", (56.85, 60.61, 16) },
                { "Казань", (55.79, 49.12, 17) },
                { "Норильск", (69.35, 88.20, 2) }
            };
        }

        public async Task<WeatherForecast> GetForecast(string region, DateTime date)
        {
            if (!_regionData.ContainsKey(region))
            {
                return GenerateSeasonalForecast(region, date);
            }

            var data = _regionData[region];
            string dateStr = date.ToString("yyyy-MM-dd");
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={data.lat}&longitude={data.lon}&daily=temperature_2m_max,temperature_2m_min,weathercode&start_date={dateStr}&end_date={dateStr}&timezone=auto";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                // Проверяем, есть ли данные
                if (json["daily"] == null)
                {
                    return GenerateSeasonalForecast(region, date);
                }

                var daily = json["daily"];
                double tempMax = daily["temperature_2m_max"][0].Value<double>();
                double tempMin = daily["temperature_2m_min"][0].Value<double>();

                if (double.IsNaN(tempMax) || double.IsInfinity(tempMax))
                {
                    return GenerateSeasonalForecast(region, date);
                }

                int temperature = (int)Math.Round((tempMax + tempMin) / 2);
                int weatherCode = daily["weathercode"][0].Value<int>();
                string condition = GetWeatherCondition(weatherCode);

                bool isAnomaly = temperature <= -30 || temperature >= 35;
                string recommendation = GetRecommendation(temperature);

                return new WeatherForecast
                {
                    Region = region,
                    Date = date,
                    Temperature = temperature,
                    Condition = condition,
                    IsAnomaly = isAnomaly,
                    Recommendation = recommendation
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка API: {ex.Message}");
                return GenerateSeasonalForecast(region, date);
            }
        }

        private WeatherForecast GenerateSeasonalForecast(string region, DateTime date)
        {
            int[] monthlyAvgTemp = { -12, -10, -4, 5, 14, 18, 20, 18, 12, 4, -4, -10 };
            int baseTemp = monthlyAvgTemp[date.Month - 1];

            if (_regionData.ContainsKey(region))
            {
                baseTemp = _regionData[region].avgTemp;
            }

            var random = new Random(date.DayOfYear + region.GetHashCode());
            int variation = random.Next(-5, 6);
            int temperature = baseTemp + variation;

            string condition = GetConditionByTemperature(temperature);
            bool isAnomaly = temperature <= -30 || temperature >= 35;
            string recommendation = GetRecommendation(temperature);

            return new WeatherForecast
            {
                Region = region,
                Date = date,
                Temperature = temperature,
                Condition = condition,
                IsAnomaly = isAnomaly,
                Recommendation = recommendation
            };
        }

        private string GetConditionByTemperature(int temp)
        {
            if (temp <= -15) return "Снег, сильный мороз";
            if (temp <= -5) return "Снег, холодно";
            if (temp <= 0) return "Мокрый снег, около нуля";
            if (temp <= 5) return "Дождь со снегом, холодно";
            if (temp <= 10) return "Облачно, прохладно";
            if (temp <= 18) return "Переменная облачность, комфортно";
            if (temp <= 25) return "Солнечно, тепло";
            if (temp <= 30) return "Жарко, возможна гроза";
            return "Сильная жара, душно";
        }

        private string GetWeatherCondition(int code)
        {
            if (code == 0) return "Ясно, солнечно";
            if (code <= 3) return "Переменная облачность";
            if (code <= 49) return "Туман";
            if (code <= 59) return "Морось";
            if (code <= 69) return "Дождь";
            if (code <= 79) return "Снег";
            if (code <= 84) return "Ливень";
            if (code <= 99) return "Гроза";
            return "Облачно";
        }

        private string GetRecommendation(int temp)
        {
            if (temp <= -30)
                return "⚠️ Резкое похолодание! Используйте термоконтейнер, оформите страховку.";
            if (temp >= 35)
                return "⚠️ Аномальная жара! Используйте охлаждающие элементы, усильте упаковку.";
            if (temp <= -15)
                return "❄️ Холодно! Рекомендуется утеплённая упаковка.";
            if (temp >= 25)
                return "☀️ Тепло! Обычная упаковка подходит.";
            return "✅ Погодные условия в норме. Дополнительных мер не требуется.";
        }
    }
}
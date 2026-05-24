using System;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Services
{
    public class WeatherForecast
    {
        public int Temperature { get; set; }
        public string Condition { get; set; }
        public bool IsAnomaly { get; set; }
        public string Recommendation { get; set; }
    }

    public class WeatherService
    {
        public async Task<WeatherForecast> GetForecast(string region, DateTime date)
        {
            await Task.Delay(500);
            var random = new Random();
            int temp = random.Next(-35, 40);
            string[] conditions = { "Солнечно", "Облачно", "Дождь", "Снег", "Туман", "Гроза", "Мороз", "Жара" };
            string condition = conditions[random.Next(conditions.Length)];

            bool isAnomaly = temp <= -30 || temp >= 35;
            string recommendation = "";
            if (temp <= -30)
                recommendation = "Резкое похолодание! Рекомендуется использовать термоконтейнер, оформите страховку от порчи товара.";
            else if (temp >= 35)
                recommendation = "Аномальная жара! Рекомендуется использовать охлаждающие элементы, усилить упаковку, оформить страховку.";
            else if (temp <= -15)
                recommendation = "Холодно! Рекомендуется утеплённая упаковка.";
            else if (temp >= 25)
                recommendation = "Тепло! Обычная упаковка подходит.";

            return new WeatherForecast
            {
                Temperature = temp,
                Condition = condition,
                IsAnomaly = isAnomaly,
                Recommendation = recommendation
            };
        }
    }
}
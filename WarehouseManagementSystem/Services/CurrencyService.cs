using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;
using WarehouseManagementSystem.Helpers;

namespace WarehouseManagementSystem.Services
{
    // Добавили связь с интерфейсом : ICurrencyService
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService()
        {
            _httpClient = new HttpClient();
        }

        // Получение курсов из API Центробанка
        public async Task<Dictionary<string, decimal>> FetchRatesFromApi()
        {
            var rates = new Dictionary<string, decimal>();
            string url = "https://www.cbr-xml-daily.ru/daily_json.js";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                using (JsonDocument document = JsonDocument.Parse(response))
                {
                    var root = document.RootElement;
                    if (root.TryGetProperty("Valute", out JsonElement valute))
                    {
                        if (valute.TryGetProperty("USD", out JsonElement usd))
                        {
                            rates["USD"] = usd.GetProperty("Value").GetDecimal();
                        }
                        if (valute.TryGetProperty("EUR", out JsonElement eur))
                        {
                            rates["EUR"] = eur.GetProperty("Value").GetDecimal();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex, "Ошибка получения курсов валют");
                return null;
            }

            return rates;
        }

        public async Task UpdateExchangeRates()
        {
            var rates = await FetchRatesFromApi();
            if (rates == null) return;

            foreach (var rate in rates)
            {
                string sql = @"INSERT INTO ExchangeRates (CurrencyCode, Rate, Date) 
                               VALUES (@code, @rate, CURRENT_DATE)
                               ON CONFLICT (CurrencyCode, Date) 
                               DO UPDATE SET Rate = EXCLUDED.Rate";

                var parameters = new[]
                {
                    new NpgsqlParameter("@code", rate.Key),
                    new NpgsqlParameter("@rate", rate.Value)
                };
                DatabaseHelper.ExecuteNonQuery(sql, parameters);
            }
        }

        // Загрузка последнего курса из базы (если интернет отключён)
        public decimal GetLastRate(string currencyCode)
        {
            string sql = "SELECT Rate FROM ExchangeRates WHERE CurrencyCode = @code ORDER BY Date DESC LIMIT 1";
            var param = new NpgsqlParameter("@code", currencyCode);
            var result = DatabaseHelper.ExecuteScalar(sql, new[] { param });
            return result != null ? Convert.ToDecimal(result) : 0;
        }
    }
}
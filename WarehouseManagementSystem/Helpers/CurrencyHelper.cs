using Npgsql;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.Helpers
{
    public static class CurrencyHelper
    {
        public static string CurrentCurrency { get; set; } = "RUB";
        public static decimal USDRate { get; set; } = 0;
        public static decimal EURRate { get; set; } = 0;

        public static void LoadSettings()
        {
            // Загружаем выбранную валюту из таблицы AppSettings
            string sql = "SELECT SettingValue FROM AppSettings WHERE SettingKey = 'BaseCurrency'";
            var result = DatabaseHelper.ExecuteScalar(sql);
            if (result != null) CurrentCurrency = result.ToString();

            // Загружаем последние курсы из базы
            var service = new CurrencyService();
            USDRate = service.GetLastRate("USD");
            EURRate = service.GetLastRate("EUR");
        }

        public static decimal Convert(decimal amountInRub)
        {
            if (CurrentCurrency == "USD" && USDRate > 0)
                return amountInRub / USDRate;
            if (CurrentCurrency == "EUR" && EURRate > 0)
                return amountInRub / EURRate;

            return amountInRub; // если рубли или ошибка
        }
    }
}
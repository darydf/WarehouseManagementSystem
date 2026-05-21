/*using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;
using WarehouseManagementSystem.Helpers;

namespace WarehouseManagementSystem.Services
{
    // Класс для хранения результата проверки
    public class ContractorCheckResult
    {
        public string Inn { get; set; }
        public string Status { get; set; }     // "RELIABLE" или "BLACKLISTED"
        public string Message { get; set; }
        public bool IsFromCache { get; set; }  // из БД или свежий?
        public DateTime CheckedAt { get; set; }
    }

    // Сервис для проверки контрагентов
    public class ContractorCheckService
    {
        private readonly HttpClient _httpClient;

        public ContractorCheckService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        // Главный метод: проверяет интернет → API → БД
        public async Task<ContractorCheckResult> CheckByInn(string inn, int userId)
        {
            // 1. Пробуем получить свежие данные через API
            if (await IsInternetAvailable())
            {
                try
                {
                    var freshResult = await FetchFromApi(inn);
                    if (freshResult != null)
                    {
                        SaveToDatabase(freshResult, userId);
                        freshResult.IsFromCache = false;
                        return freshResult;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"API error: {ex.Message}");
                }
            }

            // 2. Нет интернета или ошибка — грузим из БД
            var cachedResult = LoadFromDatabase(inn);
            if (cachedResult != null)
            {
                cachedResult.IsFromCache = true;
                cachedResult.Message = "[КЭШ] " + cachedResult.Message;
                return cachedResult;
            }

            // 3. Нет данных и нет интернета
            return new ContractorCheckResult
            {
                Inn = inn,
                Status = "UNKNOWN",
                Message = "Нет данных о контрагенте и нет интернета для проверки.",
                CheckedAt = DateTime.Now
            };
        }

        // Проверка интернета (пинг до Google DNS)
        private async Task<bool> IsInternetAvailable()
        {
            try
            {
                var ping = new System.Net.NetworkInformation.Ping();
                var reply = await ping.SendPingAsync("8.8.8.8", 2000);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        // Запрос к API (пока демо-режим)
        private async Task<ContractorCheckResult> FetchFromApi(string inn)
        {
            // Имитация задержки сети
            await Task.Delay(500);

            // Демо-режим: только тестовые ИНН дают "RELIABLE"
            // Реальные ИНН для теста: 7707083893 (Сбер), 7736050003 (Газпром)
            var reliableInns = new[] { "7707083893", "7736050003" };

            if (Array.Exists(reliableInns, i => i == inn))
            {
                return new ContractorCheckResult
                {
                    Inn = inn,
                    Status = "RELIABLE",
                    Message = "Контрагент надёжный. Можно продолжать оформление.",
                    CheckedAt = DateTime.Now
                };
            }
            else
            {
                return new ContractorCheckResult
                {
                    Inn = inn,
                    Status = "BLACKLISTED",
                    Message = "Контрагент в чёрном списке! Оформление запрещено.",
                    CheckedAt = DateTime.Now
                };
            }
        }

        // Сохранение результата в БД
        private void SaveToDatabase(ContractorCheckResult result, int userId)
        {
            string sql = @"
                INSERT INTO ContractorChecks (Inn, Status, Message, CheckedAt, CheckedByUserId)
                VALUES (@inn, @status, @message, @checkedAt, @userId)";

            var parameters = new[]
            {
                new NpgsqlParameter("@inn", result.Inn),
                new NpgsqlParameter("@status", result.Status),
                new NpgsqlParameter("@message", result.Message),
                new NpgsqlParameter("@checkedAt", result.CheckedAt),
                new NpgsqlParameter("@userId", userId)
            };
            DatabaseHelper.ExecuteNonQuery(sql, parameters);
        }

        // Загрузка последней проверки из БД
        private ContractorCheckResult LoadFromDatabase(string inn)
        {
            string sql = "SELECT Inn, Status, Message, CheckedAt FROM ContractorChecks WHERE Inn = @inn ORDER BY CheckedAt DESC LIMIT 1";
            var data = DatabaseHelper.ExecuteQuery(sql, new[] { new NpgsqlParameter("@inn", inn) });

            if (data.Rows.Count == 0) return null;

            var row = data.Rows[0];
            return new ContractorCheckResult
            {
                Inn = row["Inn"].ToString(),
                Status = row["Status"].ToString(),
                Message = row["Message"].ToString(),
                CheckedAt = Convert.ToDateTime(row["CheckedAt"])
            };
        }
    }
} */
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;
using WarehouseManagementSystem.Helpers;

namespace WarehouseManagementSystem.Services
{
    // Класс для хранения результата проверки
    public class ContractorCheckResult
    {
        public string Inn { get; set; }
        public string Status { get; set; }        // "RELIABLE" или "BLACKLISTED" или "UNKNOWN"
        public string Message { get; set; }
        public bool IsFromCache { get; set; }      // из БД или свежий?
        public DateTime CheckedAt { get; set; }
        public string CounterpartyName { get; set; }
        public bool HasTaxDebt { get; set; }
        public bool IsBankrupt { get; set; }
        public bool IsDirectorDisqual { get; set; }
    }

    // Сервис для проверки контрагентов
    public class ContractorCheckService
    {
        private readonly HttpClient _httpClient;

        public ContractorCheckService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        // Главный метод: проверяет интернет → API → БД
        public async Task<ContractorCheckResult> CheckByInn(string inn, int userId)
        {
            // 1. Пробуем получить свежие данные через API
            if (await IsInternetAvailable())
            {
                try
                {
                    var freshResult = await FetchFromApi(inn);
                    if (freshResult != null)
                    {
                        SaveToDatabase(freshResult, userId);
                        freshResult.IsFromCache = false;
                        return freshResult;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"API error: {ex.Message}");
                }
            }

            // 2. Нет интернета или ошибка — грузим из БД
            var cachedResult = LoadFromDatabase(inn);
            if (cachedResult != null)
            {
                cachedResult.IsFromCache = true;
                cachedResult.Message = "[КЭШ] " + cachedResult.Message;
                return cachedResult;
            }

            // 3. Нет данных и нет интернета
            return new ContractorCheckResult
            {
                Inn = inn,
                Status = "UNKNOWN",
                Message = "Нет данных о контрагенте и нет интернета для проверки.",
                CheckedAt = DateTime.Now
            };
        }

        // Проверка интернета (пинг до Google DNS)
        private async Task<bool> IsInternetAvailable()
        {
            try
            {
                var ping = new System.Net.NetworkInformation.Ping();
                var reply = await ping.SendPingAsync("8.8.8.8", 2000);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        // Запрос к API (демо-режим с тестовыми ИНН)
        private async Task<ContractorCheckResult> FetchFromApi(string inn)
        {
            // Имитация задержки сети
            await Task.Delay(500);

            // Демо-режим: только тестовые ИНН дают "RELIABLE"
            // Реальные ИНН для теста: 7707083893 (Сбер), 7736050003 (Газпром)
            var reliableInns = new[] { "7707083893", "7736050003" };

            if (Array.Exists(reliableInns, i => i == inn))
            {
                return new ContractorCheckResult
                {
                    Inn = inn,
                    Status = "RELIABLE",
                    Message = "Контрагент надёжный. Можно продолжать оформление.",
                    CounterpartyName = inn == "7707083893" ? "Сбербанк" : "Газпром",
                    HasTaxDebt = false,
                    IsBankrupt = false,
                    IsDirectorDisqual = false,
                    CheckedAt = DateTime.Now
                };
            }
            else if (inn.Length == 10 || inn.Length == 12)
            {
                // Остальные ИНН — с рисками (демонстрация красного статуса)
                return new ContractorCheckResult
                {
                    Inn = inn,
                    Status = "BLACKLISTED",
                    Message = "Контрагент в чёрном списке! Оформление запрещено.",
                    CounterpartyName = "Неизвестная компания",
                    HasTaxDebt = true,
                    IsBankrupt = true,
                    IsDirectorDisqual = true,
                    CheckedAt = DateTime.Now
                };
            }
            else
            {
                return new ContractorCheckResult
                {
                    Inn = inn,
                    Status = "UNKNOWN",
                    Message = "Некорректный ИНН. Введите 10 или 12 цифр.",
                    CheckedAt = DateTime.Now
                };
            }
        }

        // Сохранение результата в БД
        private void SaveToDatabase(ContractorCheckResult result, int userId)
        {
            try
            {
                string sql = @"
                    INSERT INTO ContractorChecks (Inn, Status, Message, CounterpartyName, HasTaxDebt, IsBankrupt, IsDirectorDisqual, CheckedAt, CheckedByUserId)
                    VALUES (@inn, @status, @message, @name, @taxDebt, @bankrupt, @directorDisqual, @checkedAt, @userId)";

                var parameters = new[]
                {
                    new NpgsqlParameter("@inn", result.Inn),
                    new NpgsqlParameter("@status", result.Status),
                    new NpgsqlParameter("@message", result.Message),
                    new NpgsqlParameter("@name", result.CounterpartyName ?? (object)DBNull.Value),
                    new NpgsqlParameter("@taxDebt", result.HasTaxDebt),
                    new NpgsqlParameter("@bankrupt", result.IsBankrupt),
                    new NpgsqlParameter("@directorDisqual", result.IsDirectorDisqual),
                    new NpgsqlParameter("@checkedAt", result.CheckedAt),
                    new NpgsqlParameter("@userId", userId)
                };
                DatabaseHelper.ExecuteNonQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения в БД: {ex.Message}");
            }
        }

        // Загрузка последней проверки из БД
        private ContractorCheckResult LoadFromDatabase(string inn)
        {
            try
            {
                string sql = @"
                    SELECT Inn, Status, Message, CounterpartyName, HasTaxDebt, IsBankrupt, IsDirectorDisqual, CheckedAt 
                    FROM ContractorChecks 
                    WHERE Inn = @inn 
                    ORDER BY CheckedAt DESC 
                    LIMIT 1";

                var data = DatabaseHelper.ExecuteQuery(sql, new[] { new NpgsqlParameter("@inn", inn) });

                if (data.Rows.Count == 0) return null;

                var row = data.Rows[0];
                return new ContractorCheckResult
                {
                    Inn = row["Inn"].ToString(),
                    Status = row["Status"].ToString(),
                    Message = row["Message"].ToString(),
                    CounterpartyName = row["CounterpartyName"].ToString(),
                    HasTaxDebt = Convert.ToBoolean(row["HasTaxDebt"]),
                    IsBankrupt = Convert.ToBoolean(row["IsBankrupt"]),
                    IsDirectorDisqual = Convert.ToBoolean(row["IsDirectorDisqual"]),
                    CheckedAt = Convert.ToDateTime(row["CheckedAt"])
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки из БД: {ex.Message}");
                return null;
            }
        }
    }
}
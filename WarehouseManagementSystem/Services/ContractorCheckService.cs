using System;
using System.Threading.Tasks;
using Npgsql;
using WarehouseManagementSystem.Helpers;
using Dadata;
using Dadata.Model;
using WarehouseManagementSystem.Interfaces;

namespace WarehouseManagementSystem.Services
{
    public class ContractorCheckResult
    {
        public string Inn { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsFromCache { get; set; }
        public DateTime CheckedAt { get; set; }
        public string CounterpartyName { get; set; }
        public bool HasTaxDebt { get; set; }
        public bool IsBankrupt { get; set; }
        public bool IsDirectorDisqual { get; set; }
    }

    public class ContractorCheckService
    {
        private readonly string _token = "c67a5a8ff92cd87e292fc46801a0e2b9b49d4419";
        private readonly SuggestClientAsync _suggestClient;

        // Для тестов (если переданы интерфейсы - используем их, иначе null)
        private readonly IContractorApiClient _testApiClient;
        private readonly IContractorRepository _testRepository;
        private readonly INetworkChecker _testNetworkChecker;
        private readonly bool _isTestMode;

        // ОРИГИНАЛЬНЫЙ КОНСТРУКТОР (работает как раньше)
        public ContractorCheckService()
        {
            _suggestClient = new SuggestClientAsync(_token);
            _isTestMode = false;
        }

        // КОНСТРУКТОР ДЛЯ ТЕСТОВ (не влияет на основную работу)
        public ContractorCheckService(
        IContractorApiClient apiClient,
        IContractorRepository repository,
        INetworkChecker networkChecker)
        {
            _testApiClient = apiClient;
            _testRepository = repository;
            _testNetworkChecker = networkChecker;
            _isTestMode = true;
            // _suggestClient останется null, просто не используем его в тестовом режиме
        }

        public async Task<ContractorCheckResult> CheckByInn(string inn, int userId)
        {
            // В ТЕСТОВОМ РЕЖИМЕ используем моки
            if (_isTestMode)
            {
                return await CheckByInnTest(inn, userId);
            }

            // ОРИГИНАЛЬНАЯ ЛОГИКА (НЕ ТРОГАЕМ)
            if (await IsInternetAvailable())
            {
                try
                {
                    var freshResult = await FetchFromDadata(inn);
                    if (freshResult != null)
                    {
                        SaveToDatabase(freshResult, userId);
                        freshResult.IsFromCache = false;
                        return freshResult;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DaData API error: {ex.Message}");
                }
            }

            var cachedResult = LoadFromDatabase(inn);
            if (cachedResult != null)
            {
                cachedResult.IsFromCache = true;
                cachedResult.Message = "[КЭШ] " + cachedResult.Message;
                return cachedResult;
            }

            return new ContractorCheckResult
            {
                Inn = inn,
                Status = "UNKNOWN",
                Message = "Нет данных о контрагенте и нет интернета для проверки.",
                CheckedAt = DateTime.Now
            };
        }

        // ТЕСТОВАЯ ВЕРСИЯ (использует моки)
        private async Task<ContractorCheckResult> CheckByInnTest(string inn, int userId)
        {
            if (await _testNetworkChecker.IsInternetAvailable())
            {
                try
                {
                    var response = await _testApiClient.FindParty(inn);
                    if (response != null)
                    {
                        var freshResult = new ContractorCheckResult
                        {
                            Inn = inn,
                            Status = "RELIABLE",
                            Message = "Контрагент найден (тест)",
                            CheckedAt = DateTime.Now
                        };
                        _testRepository.SaveCheckResult(freshResult, userId);
                        freshResult.IsFromCache = false;
                        return freshResult;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Test API error: {ex.Message}");
                }
            }

            var cachedResult = _testRepository.LoadLatestByInn(inn);
            if (cachedResult != null)
            {
                cachedResult.IsFromCache = true;
                cachedResult.Message = "[КЭШ] " + cachedResult.Message;
                return cachedResult;
            }

            return new ContractorCheckResult
            {
                Inn = inn,
                Status = "UNKNOWN",
                Message = "Нет данных о контрагенте и нет интернета для проверки.",
                CheckedAt = DateTime.Now
            };
        }

        // ОРИГИНАЛЬНЫЙ метод проверки интернета (НЕ ТРОГАЕМ)
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

        // ОРИГИНАЛЬНЫЙ метод FetchFromDadata (НЕ ТРОГАЕМ - полностью твой код)
        private async Task<ContractorCheckResult> FetchFromDadata(string inn)
        {
            try
            {
                var response = await _suggestClient.FindParty(inn);

                if (response == null || response.suggestions == null || response.suggestions.Count == 0)
                {
                    return new ContractorCheckResult
                    {
                        Inn = inn,
                        Status = "UNKNOWN",
                        Message = "Контрагент не найден в базе ФНС",
                        CheckedAt = DateTime.Now
                    };
                }

                var party = response.suggestions[0].data;

                string fullName = "Неизвестно";
                if (party.name != null)
                {
                    if (!string.IsNullOrEmpty(party.name.full_with_opf))
                        fullName = party.name.full_with_opf;
                    else if (!string.IsNullOrEmpty(party.name.full))
                        fullName = party.name.full;
                }

                bool isActive = false;
                bool isLiquidated = false;
                bool isBankrupt = false;

                if (party.state != null)
                {
                    var status = party.state.status;
                    isActive = (status == PartyStatus.ACTIVE);
                    isLiquidated = (status == PartyStatus.LIQUIDATED);
                    isBankrupt = (status == PartyStatus.BANKRUPT);
                }

                string resultStatus = (isActive && !isLiquidated && !isBankrupt) ? "RELIABLE" : "BLACKLISTED";
                string message = resultStatus == "RELIABLE"
                    ? $"Контрагент надёжный. {fullName}"
                    : $"Контрагент в чёрном списке!";

                return new ContractorCheckResult
                {
                    Inn = inn,
                    Status = resultStatus,
                    Message = message,
                    CounterpartyName = fullName,
                    HasTaxDebt = false,
                    IsBankrupt = isBankrupt,
                    IsDirectorDisqual = false,
                    CheckedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DaData API error: {ex.Message}");
                return new ContractorCheckResult
                {
                    Inn = inn,
                    Status = "ERROR",
                    Message = "Ошибка при проверке контрагента",
                    CheckedAt = DateTime.Now
                };
            }
        }

        // ОРИГИНАЛЬНЫЙ метод сохранения (НЕ ТРОГАЕМ)
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

        // ОРИГИНАЛЬНЫЙ метод загрузки (НЕ ТРОГАЕМ)
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
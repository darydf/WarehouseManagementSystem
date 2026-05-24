using System.Collections.Generic;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Services
{
    public interface ICurrencyService
    {
        Task<Dictionary<string, decimal>> FetchRatesFromApi();
        Task UpdateExchangeRates();
        decimal GetLastRate(string currencyCode);
    }
}

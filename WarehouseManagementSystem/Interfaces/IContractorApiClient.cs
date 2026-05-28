using System.Threading.Tasks;

namespace WarehouseManagementSystem.Interfaces
{
    public interface IContractorApiClient
    {
        Task<object> FindParty(string inn);
    }
}
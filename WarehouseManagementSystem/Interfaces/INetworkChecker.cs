using System.Threading.Tasks;

namespace WarehouseManagementSystem.Interfaces
{
    public interface INetworkChecker
    {
        Task<bool> IsInternetAvailable();
    }
}
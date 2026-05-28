using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.Interfaces
{
    public interface IContractorRepository
    {
        void SaveCheckResult(ContractorCheckResult result, int userId);
        ContractorCheckResult LoadLatestByInn(string inn);
    }
}
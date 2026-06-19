using System.Data;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Services
{
    public interface IProductService
    {
        DataTable GetAllProducts(string searchText = "");
        Product GetProductById(int id);
        bool IsArticleUnique(string article, int? excludeId = null);
        int AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
        DataTable GetProductsForShipment(string searchText = "");
    }
}

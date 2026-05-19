using System;
using System.Data;

namespace WarehouseManagementSystem.Services
{
    public interface ICategoryService
    {
        DataTable GetAllCategories();
        DataRow GetCategoryById(int id);
        int AddCategory(string name, string description);
        void UpdateCategory(int id, string name, string description);
        void DeleteCategory(int id);
        DataTable GetCategoriesForCombo();
    }
}
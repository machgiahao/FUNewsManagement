using FUNewsManagementSystem.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        void SaveCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
        Category GetCategoryById(int id);
    }
}

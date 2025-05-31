using FUNewsManagementSystem.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface ICategoryService
    {
        List<Category> GetCategories();
        public void CreateCategory(Category Category);
        public void UpdateCategory(Category Category);
        public void DeleteCategory(int newsArticleId);
        Category GetCategoryById(int id);
    }
}

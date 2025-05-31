
using FUNewsManagementSystem.BusinessObject;
using FUNewsManagementSystem.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository iCategoryRepository;
        public CategoryService()
        {
            this.iCategoryRepository = new CategoryRepository();
        }
        public List<Category> GetCategories()
        {
           return iCategoryRepository.GetCategories();
        }

        public void DeleteCategory(int newsArticleId) => iCategoryRepository.DeleteCategory(newsArticleId);

        public void CreateCategory(Category Category) => iCategoryRepository.SaveCategory(Category);

        public void UpdateCategory(Category Category) => iCategoryRepository.UpdateCategory(Category);
        public Category GetCategoryById(int id) => iCategoryRepository.GetCategoryById(id);
    }
}

using FUNewsManagementSystem.BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public class CategoryRepository : ICategoryRepository
    {

        public List<Category> GetCategories()
        {
            var listCategories = new List<Category>();
            try
            {
                using var context = new FunewsManagementContext();
                listCategories = context.Categories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCategories: " + ex.Message);

            }
            return listCategories;
        }

        public Category GetCategoryById(int id)
        {
            using var context = new FunewsManagementContext();
            return context.Categories
                .Include(c => c.ParentCategory) 
                .Include(c => c.NewsArticles)
                .FirstOrDefault(na => na.CategoryId == id);
        }

        public void SaveCategory(Category category)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveCategory: " + ex.Message);
            }
        }

        public void UpdateCategory(Category category)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry<Category>(category).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateCategory: " + ex.Message);
            }
        }
        public void DeleteCategory(int id)
        {
            try
            {
                using var context = new FunewsManagementContext();

                // Find the category to delete
                var category = context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null)
                {
                    throw new Exception("Category not found");
                }

                // Find and update all child categories
                var childCategories = context.Categories.Where(c => c.ParentCategoryId == id).ToList();
                foreach (var child in childCategories)
                {
                    child.ParentCategoryId = null;
                }

                // Remove the category
                context.Categories.Remove(category);

                // Save all changes
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteCategory: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}

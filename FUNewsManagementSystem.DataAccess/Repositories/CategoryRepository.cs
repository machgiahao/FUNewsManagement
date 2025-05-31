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
            return context.Categories.FirstOrDefault(na => na.CategoryId.Equals(id));
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
                var category = context.Categories.SingleOrDefault(na => na.CategoryId.Equals(id));
                context.Categories.Remove(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteCategory: " + ex.Message);
            }
        }
    }
}

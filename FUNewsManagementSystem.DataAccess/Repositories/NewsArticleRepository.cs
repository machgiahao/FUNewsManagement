using FUNewsManagementSystem.BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public List<NewsArticle> GetNewsArticles()
        {
            var listNewsArticles = new List<NewsArticle>();
            try
            {
                using var context = new FunewsManagementContext();
                listNewsArticles = context.NewsArticles.Include(na => na.Category).Include(na => na.Tags).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsArticles: " + ex.Message);
            }
            return listNewsArticles;
        }

        public void SaveNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                using var context = new FunewsManagementContext();
                
                if (newsArticle.Tags != null && newsArticle.Tags.Any())
                {
                    var tagIds = newsArticle.Tags.Select(t => t.TagId).ToList();
                    var existingTags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                    newsArticle.Tags = existingTags;
                }
                context.NewsArticles.Add(newsArticle);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("Error in SaveNewsArticle: " + ex.Message);
            }
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                using var context = new FunewsManagementContext();

                // Load the existing article including its tags
                var existingArticle = context.NewsArticles
                    .Include(na => na.Tags)
                    .FirstOrDefault(na => na.NewsArticleId == newsArticle.NewsArticleId);

                if (existingArticle == null)
                    throw new Exception("NewsArticle not found.");

                // Preserve CreatedDate and CreatedById
                var createdDate = existingArticle.CreatedDate;
                var createdById = existingArticle.CreatedById;

                // Update scalar properties
                context.Entry(existingArticle).CurrentValues.SetValues(newsArticle);

                // Restore CreatedDate and CreatedById
                existingArticle.CreatedDate = createdDate;
                existingArticle.CreatedById = createdById;

                // Synchronize tags (your existing logic)
                var newTagIds = newsArticle.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();
                var currentTagIds = existingArticle.Tags.Select(t => t.TagId).ToList();

                var tagsToRemove = existingArticle.Tags.Where(t => !newTagIds.Contains(t.TagId)).ToList();
                foreach (var tag in tagsToRemove)
                {
                    existingArticle.Tags.Remove(tag);
                }

                var tagsToAddIds = newTagIds.Except(currentTagIds).ToList();
                if (tagsToAddIds.Any())
                {
                    var tagsToAdd = context.Tags.Where(t => tagsToAddIds.Contains(t.TagId)).ToList();
                    foreach (var tag in tagsToAdd)
                    {
                        existingArticle.Tags.Add(tag);
                    }
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateNewsArticle: " + ex.Message);
            }
        }


        public void DeleteNewsArticle(string newsArticleId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var newsArticle = context.NewsArticles
                    .Include(na => na.Tags)
                    .FirstOrDefault(na => na.NewsArticleId.Equals(newsArticleId));
                if (newsArticle != null)
                {
                    // Detach tags to avoid foreign key constraint issues
                    newsArticle.Tags.Clear();
                    context.NewsArticles.Remove(newsArticle);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteNewsArticle: " + ex.Message);
            }
        }


        public NewsArticle GetNewsArticleById(string newsArticleId)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles.FirstOrDefault(na => na.NewsArticleId.Equals(newsArticleId));
        }

        public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.NewsArticles
                    .Include(na => na.Category)
                    .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsArticlesByPeriod: " + ex.Message);
            }
        }
    }
}
 
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
                listNewsArticles = context.NewsArticles.Include(na => na.Category).ToList();
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
                if (newsArticle.Tags != null && newsArticle.Tags.Any())
                {
                    var tagIds = newsArticle.Tags.Select(t => t.TagId).ToList();
                    var existingTags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                    newsArticle.Tags = existingTags;
                }
                context.Entry<NewsArticle>(newsArticle).State = EntityState.Modified;
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
                var newsArticle = context.NewsArticles.SingleOrDefault(na => na.NewsArticleId.Equals(newsArticleId));
                context.NewsArticles.Remove(newsArticle);
                context.SaveChanges();
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

        public List<NewsArticle> GetListNewsArticlesByCreator(int creatorId)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.CreatedById == creatorId)
                .ToList();
        }

        public NewsArticle GetDetailNewsArticleById(string newsArticleId)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Include(n => n.CreatedBy)
                .Include(n => n.UpdatedBy)
                .FirstOrDefault(n => n.NewsArticleId == newsArticleId);
        }
    }
}
 
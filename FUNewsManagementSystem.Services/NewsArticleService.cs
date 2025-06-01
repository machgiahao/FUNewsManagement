
using FUNewsManagementSystem.BusinessObject;
using FUNewsManagementSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _iNewsArticleRepository;
        private readonly IConfiguration _config;

        public NewsArticleService(IAccountRepository iAccountRepository, IConfiguration config)
        {
            this._iNewsArticleRepository = new NewsArticleRepository();
            this._config = config;
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return _iNewsArticleRepository.GetNewsArticles();
        }

        public void SaveNewsArticle(NewsArticle newsArticle)
        {
            var createdById = _config["AdminAccount:Email"];
            _iNewsArticleRepository.SaveNewsArticle(newsArticle);
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            _iNewsArticleRepository.UpdateNewsArticle(newsArticle);
        }

        public void DeleteNewsArticle(string newsArticleId)
        {
            _iNewsArticleRepository.DeleteNewsArticle(newsArticleId);
        }

        public NewsArticle GetNewsArticleById(string newsArticleId)
        {
            return _iNewsArticleRepository.GetNewsArticleById(newsArticleId);
        }

        public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate)
        {
            return _iNewsArticleRepository.GetNewsArticlesByPeriod(startDate, endDate);
        }

        public List<NewsArticle> SearchNewsArticles(string searchField, string searchString, int? tagId = null)
        {
            var articles = GetNewsArticles();

            // Filter by tag if tagId is provided
            if (tagId.HasValue && tagId.Value > 0)
            {
                articles = articles.Where(a => a.Tags != null && a.Tags.Any(t => t.TagId == tagId.Value)).ToList();
            }

            // Filter by search field/string if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchField)
                {
                    case "Headline":
                        articles = articles.Where(a => a.Headline != null && a.Headline.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "NewsSource":
                        articles = articles.Where(a => a.NewsSource != null && a.NewsSource.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "CategoryName":
                        articles = articles.Where(a => a.Category != null && a.Category.CategoryName != null && a.Category.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default: // NewsTitle
                        articles = articles.Where(a => a.NewsTitle != null && a.NewsTitle.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                }
            }

            return articles;
        }



        public List<NewsArticle> GetListNewsArticlesByCreator(int creatorId)
        {
            return _iNewsArticleRepository.GetListNewsArticlesByCreator(creatorId);
        }

        public NewsArticle GetDetailNewsArticleById(string newArticleId)
        {
            return _iNewsArticleRepository.GetDetailNewsArticleById(newArticleId);
        }
    }
}

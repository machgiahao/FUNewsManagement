
using FUNewsManagementSystem.BusinessObject;
using FUNewsManagementSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository iNewsArticleRepository;

        public NewsArticleService()
        {
            this.iNewsArticleRepository = new NewsArticleRepository();
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return iNewsArticleRepository.GetNewsArticles();
        }

        public void SaveNewsArticle(NewsArticle newsArticle)
        {
            iNewsArticleRepository.SaveNewsArticle(newsArticle);
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            iNewsArticleRepository.UpdateNewsArticle(newsArticle);
        }

        public void DeleteNewsArticle(string newsArticleId)
        {
            iNewsArticleRepository.DeleteNewsArticle(newsArticleId);
        }

        public NewsArticle GetNewsArticleById(string newsArticleId)
        {
            return iNewsArticleRepository.GetNewsArticleById(newsArticleId);
        }

        public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate)
        {
            return iNewsArticleRepository.GetNewsArticlesByPeriod(startDate, endDate);
        }

    }
}

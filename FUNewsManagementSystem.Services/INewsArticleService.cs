using FUNewsManagementSystem.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface INewsArticleService
    {
        List<NewsArticle> GetNewsArticles();
        void SaveNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        void DeleteNewsArticle(string newsArticleId);
        NewsArticle GetNewsArticleById(string newsArticleId);
        List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate);
    }
}


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

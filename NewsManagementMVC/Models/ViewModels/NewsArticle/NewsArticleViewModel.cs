using System;
using EntityNewsArticle = FUNewsManagementSystem.BusinessObject.NewsArticle;
namespace NewsManagementMVC.Models.ViewModels.NewsArticle
{
    public class NewsArticleViewModel
    {
        public string NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public bool? NewsStatus { get; set; }
        public string CategoryName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<int> SelectedTagIds { get; set; } = new();
        public static NewsArticleViewModel FromNewsArticle(EntityNewsArticle n)
        {
            return new NewsArticleViewModel
            {
                NewsArticleId = n.NewsArticleId,
                NewsTitle = n.NewsTitle,
                Headline = n.Headline,
                NewsContent = n.NewsContent,
                NewsSource = n.NewsSource,
                NewsStatus = n.NewsStatus,
                CategoryName = n.Category?.CategoryName
            };
        }
    }
}

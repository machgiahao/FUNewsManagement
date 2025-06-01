namespace NewsManagementMVC.Models.ViewModels.NewsArticle
{
    public class NewsArticleSummaryViewModel
    {
        public string NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CategoryName { get; set; }
        public bool? NewsStatus { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

}

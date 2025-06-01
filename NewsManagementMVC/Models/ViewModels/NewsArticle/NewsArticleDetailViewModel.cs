namespace NewsManagementMVC.Models.ViewModels.NewsArticle
{
    public class NewsArticleDetailViewModel
    {
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public string CategoryName { get; set; }
        public bool? NewsStatus { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<string> TagNames { get; set; }
    }

}

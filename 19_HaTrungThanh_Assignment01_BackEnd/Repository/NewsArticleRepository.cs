using _19_HaTrungThanh_Assignment01_BackEnd.DAO;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Repository
{
    public interface INewsArticleRepository
    {
        Task<List<NewsArticle>> GetNewsArticlesAsync();
        Task<List<NewsArticle>> GetActiveNewsArticlesAsync();
        Task<NewsArticle?> GetNewsArticleByIdAsync(string articleId);
        Task<List<NewsArticle>> GetNewsArticlesByCreatorAsync(short creatorId);
        Task<NewsArticle> AddNewsArticleAsync(NewsArticle article, List<int>? tagIds);
        Task<bool> UpdateNewsArticleAsync(NewsArticle article, List<int>? tagIds);
        Task<(bool Success, string Message)> DeleteNewsArticleAsync(string articleId);
        Task<List<NewsArticle>> SearchNewsArticlesAsync(string keyword);
        Task<List<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate);
    }

    public class NewsArticleRepository : INewsArticleRepository
    {
        public Task<List<NewsArticle>> GetNewsArticlesAsync() => NewsArticleDAO.Instance.GetNewsArticlesAsync();
        public Task<List<NewsArticle>> GetActiveNewsArticlesAsync() => NewsArticleDAO.Instance.GetActiveNewsArticlesAsync();
        public Task<NewsArticle?> GetNewsArticleByIdAsync(string articleId) => NewsArticleDAO.Instance.GetNewsArticleByIdAsync(articleId);
        public Task<List<NewsArticle>> GetNewsArticlesByCreatorAsync(short creatorId) => NewsArticleDAO.Instance.GetNewsArticlesByCreatorAsync(creatorId);
        public Task<NewsArticle> AddNewsArticleAsync(NewsArticle article, List<int>? tagIds) => NewsArticleDAO.Instance.AddNewsArticleAsync(article, tagIds);
        public Task<bool> UpdateNewsArticleAsync(NewsArticle article, List<int>? tagIds) => NewsArticleDAO.Instance.UpdateNewsArticleAsync(article, tagIds);
        public Task<(bool Success, string Message)> DeleteNewsArticleAsync(string articleId) => NewsArticleDAO.Instance.DeleteNewsArticleAsync(articleId);
        public Task<List<NewsArticle>> SearchNewsArticlesAsync(string keyword) => NewsArticleDAO.Instance.SearchNewsArticlesAsync(keyword);
        public Task<List<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate) => NewsArticleDAO.Instance.GetReportAsync(startDate, endDate);
    }
}

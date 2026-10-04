using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _19_HaTrungThanh_Assignment01_BackEnd.DAO
{
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private NewsArticleDAO() { }

        public static NewsArticleDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NewsArticleDAO();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags!)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<NewsArticle>> GetActiveNewsArticlesAsync()
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags!)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.NewsStatus == true)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<NewsArticle?> GetNewsArticleByIdAsync(string articleId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags!)
                    .ThenInclude(nt => nt.Tag)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NewsArticleID == articleId);
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByCreatorAsync(short creatorId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags!)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedByID == creatorId)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<NewsArticle> AddNewsArticleAsync(NewsArticle article, List<int>? tagIds)
        {
            using var context = new FUNewsManagementDbContext();
            article.CreatedDate ??= DateTime.Now;
            article.ModifiedDate = DateTime.Now;

            context.NewsArticles.Add(article);
            await context.SaveChangesAsync();

            if (tagIds != null && tagIds.Any())
            {
                foreach (var tagId in tagIds.Distinct())
                {
                    context.NewsTags.Add(new NewsTag
                    {
                        NewsArticleID = article.NewsArticleID,
                        TagID = tagId
                    });
                }
                await context.SaveChangesAsync();
            }

            return article;
        }

        public async Task<bool> UpdateNewsArticleAsync(NewsArticle article, List<int>? tagIds)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefaultAsync(n => n.NewsArticleID == article.NewsArticleID);

            if (existing == null) return false;

            existing.NewsTitle = article.NewsTitle;
            existing.Headline = article.Headline;
            existing.NewsContent = article.NewsContent;
            existing.NewsSource = article.NewsSource;
            existing.CategoryID = article.CategoryID;
            existing.NewsStatus = article.NewsStatus;
            existing.UpdatedByID = article.UpdatedByID;
            existing.ModifiedDate = DateTime.Now;

            if (tagIds != null)
            {
                var currentTags = context.NewsTags.Where(nt => nt.NewsArticleID == article.NewsArticleID);
                context.NewsTags.RemoveRange(currentTags);

                foreach (var tagId in tagIds.Distinct())
                {
                    context.NewsTags.Add(new NewsTag
                    {
                        NewsArticleID = article.NewsArticleID,
                        TagID = tagId
                    });
                }
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> DeleteNewsArticleAsync(string articleId)
        {
            using var context = new FUNewsManagementDbContext();
            var article = await context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefaultAsync(n => n.NewsArticleID == articleId);

            if (article == null)
            {
                return (false, "News article not found.");
            }

            if (article.NewsTags != null && article.NewsTags.Any())
            {
                context.NewsTags.RemoveRange(article.NewsTags);
            }

            context.NewsArticles.Remove(article);
            await context.SaveChangesAsync();
            return (true, "News article deleted successfully.");
        }

        public async Task<List<NewsArticle>> SearchNewsArticlesAsync(string keyword)
        {
            using var context = new FUNewsManagementDbContext();
            var query = context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags!)
                    .ThenInclude(nt => nt.Tag)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(n => (n.NewsTitle != null && n.NewsTitle.Contains(keyword)) ||
                                         n.Headline.Contains(keyword) ||
                                         (n.NewsContent != null && n.NewsContent.Contains(keyword)) ||
                                         (n.Category != null && n.Category.CategoryName.Contains(keyword)));
            }

            return await query.OrderByDescending(n => n.CreatedDate).ToListAsync();
        }

        public async Task<List<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate)
        {
            using var context = new FUNewsManagementDbContext();
            DateTime endOfDay = (endDate >= DateTime.MaxValue.Date) ? DateTime.MaxValue : endDate.Date.AddDays(1).AddTicks(-1);

            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Where(n => n.CreatedDate >= startDate.Date && n.CreatedDate <= endOfDay)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

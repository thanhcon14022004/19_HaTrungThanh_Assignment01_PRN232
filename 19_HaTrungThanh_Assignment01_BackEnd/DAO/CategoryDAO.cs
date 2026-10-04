using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _19_HaTrungThanh_Assignment01_BackEnd.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new CategoryDAO();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using var context = new FUNewsManagementDbContext();
            return await context.Categories
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(short categoryId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.NewsArticles)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            using var context = new FUNewsManagementDbContext();
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.Categories.FindAsync(category.CategoryID);
            if (existing == null) return false;

            existing.CategoryName = category.CategoryName;
            existing.CategoryDesciption = category.CategoryDesciption;
            existing.ParentCategoryID = category.ParentCategoryID;
            existing.IsActive = category.IsActive;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(short categoryId)
        {
            using var context = new FUNewsManagementDbContext();
            var category = await context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return (false, "Category not found.");
            }

            // Business rule: If category is used by any news article, CANNOT delete
            bool hasArticles = await context.NewsArticles.AnyAsync(n => n.CategoryID == categoryId);
            if (hasArticles)
            {
                return (false, "Cannot delete category because it is already associated with one or more news articles.");
            }

            // Check if any subcategories reference this category
            bool hasSubCategories = await context.Categories.AnyAsync(c => c.ParentCategoryID == categoryId && c.CategoryID != categoryId);
            if (hasSubCategories)
            {
                return (false, "Cannot delete category because it has sub-categories referencing it.");
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return (true, "Category deleted successfully.");
        }

        public async Task<List<Category>> SearchCategoriesAsync(string keyword)
        {
            using var context = new FUNewsManagementDbContext();
            var query = context.Categories.Include(c => c.ParentCategory).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c => c.CategoryName.Contains(keyword) || c.CategoryDesciption.Contains(keyword));
            }

            return await query.ToListAsync();
        }
    }
}

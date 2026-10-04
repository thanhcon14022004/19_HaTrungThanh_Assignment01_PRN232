using _19_HaTrungThanh_Assignment01_BackEnd.DAO;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(short categoryId);
        Task<Category> AddCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<(bool Success, string Message)> DeleteCategoryAsync(short categoryId);
        Task<List<Category>> SearchCategoriesAsync(string keyword);
    }

    public class CategoryRepository : ICategoryRepository
    {
        public Task<List<Category>> GetCategoriesAsync() => CategoryDAO.Instance.GetCategoriesAsync();
        public Task<Category?> GetCategoryByIdAsync(short categoryId) => CategoryDAO.Instance.GetCategoryByIdAsync(categoryId);
        public Task<Category> AddCategoryAsync(Category category) => CategoryDAO.Instance.AddCategoryAsync(category);
        public Task<bool> UpdateCategoryAsync(Category category) => CategoryDAO.Instance.UpdateCategoryAsync(category);
        public Task<(bool Success, string Message)> DeleteCategoryAsync(short categoryId) => CategoryDAO.Instance.DeleteCategoryAsync(categoryId);
        public Task<List<Category>> SearchCategoriesAsync(string keyword) => CategoryDAO.Instance.SearchCategoriesAsync(keyword);
    }
}

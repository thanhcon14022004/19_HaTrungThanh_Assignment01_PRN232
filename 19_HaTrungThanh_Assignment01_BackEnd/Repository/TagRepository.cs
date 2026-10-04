using _19_HaTrungThanh_Assignment01_BackEnd.DAO;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Repository
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsAsync();
        Task<Tag?> GetTagByIdAsync(int tagId);
    }

    public class TagRepository : ITagRepository
    {
        public Task<List<Tag>> GetTagsAsync() => TagDAO.Instance.GetTagsAsync();
        public Task<Tag?> GetTagByIdAsync(int tagId) => TagDAO.Instance.GetTagByIdAsync(tagId);
    }
}

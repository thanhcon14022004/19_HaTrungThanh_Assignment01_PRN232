using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _19_HaTrungThanh_Assignment01_BackEnd.DAO
{
    public class TagDAO
    {
        private static TagDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private TagDAO() { }

        public static TagDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TagDAO();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            using var context = new FUNewsManagementDbContext();
            return await context.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(int tagId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.TagID == tagId);
        }
    }
}

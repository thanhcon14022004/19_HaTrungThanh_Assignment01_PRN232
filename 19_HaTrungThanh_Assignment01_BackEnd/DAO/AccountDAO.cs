using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _19_HaTrungThanh_Assignment01_BackEnd.DAO
{
    public class AccountDAO
    {
        private static AccountDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private AccountDAO() { }

        public static AccountDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new AccountDAO();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<SystemAccount>> GetAccountsAsync()
        {
            using var context = new FUNewsManagementDbContext();
            return await context.SystemAccounts
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(short accountId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.SystemAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountID == accountId);
        }

        public async Task<SystemAccount?> GetAccountByEmailAsync(string email)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.SystemAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountEmail != null && a.AccountEmail.ToLower() == email.Trim().ToLower());
        }

        public async Task<SystemAccount> AddAccountAsync(SystemAccount account)
        {
            using var context = new FUNewsManagementDbContext();
            if (account.AccountID == 0)
            {
                short maxId = await context.SystemAccounts.AnyAsync()
                    ? await context.SystemAccounts.MaxAsync(a => a.AccountID)
                    : (short)0;
                account.AccountID = (short)(maxId + 1);
            }

            context.SystemAccounts.Add(account);
            await context.SaveChangesAsync();
            return account;
        }

        public async Task<bool> UpdateAccountAsync(SystemAccount account)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.SystemAccounts.FindAsync(account.AccountID);
            if (existing == null) return false;

            existing.AccountName = account.AccountName;
            existing.AccountEmail = account.AccountEmail;
            if (account.AccountRole.HasValue)
            {
                existing.AccountRole = account.AccountRole;
            }
            if (!string.IsNullOrEmpty(account.AccountPassword))
            {
                existing.AccountPassword = account.AccountPassword;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> DeleteAccountAsync(short accountId)
        {
            using var context = new FUNewsManagementDbContext();
            var account = await context.SystemAccounts.FindAsync(accountId);
            if (account == null)
            {
                return (false, "Account not found.");
            }

            // Business rule: If account has created any news article, CANNOT delete
            bool hasArticles = await context.NewsArticles.AnyAsync(n => n.CreatedByID == accountId);
            if (hasArticles)
            {
                return (false, "Cannot delete account because this account has already created one or more news articles.");
            }

            context.SystemAccounts.Remove(account);
            await context.SaveChangesAsync();
            return (true, "Account deleted successfully.");
        }

        public async Task<List<SystemAccount>> SearchAccountsAsync(string keyword)
        {
            using var context = new FUNewsManagementDbContext();
            var query = context.SystemAccounts.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a => (a.AccountName != null && a.AccountName.Contains(keyword)) ||
                                         (a.AccountEmail != null && a.AccountEmail.Contains(keyword)));
            }

            return await query.ToListAsync();
        }
    }
}

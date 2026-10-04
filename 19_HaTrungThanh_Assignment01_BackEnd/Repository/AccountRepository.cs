using _19_HaTrungThanh_Assignment01_BackEnd.DAO;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Repository
{
    public interface IAccountRepository
    {
        Task<List<SystemAccount>> GetAccountsAsync();
        Task<SystemAccount?> GetAccountByIdAsync(short accountId);
        Task<SystemAccount?> GetAccountByEmailAsync(string email);
        Task<SystemAccount> AddAccountAsync(SystemAccount account);
        Task<bool> UpdateAccountAsync(SystemAccount account);
        Task<(bool Success, string Message)> DeleteAccountAsync(short accountId);
        Task<List<SystemAccount>> SearchAccountsAsync(string keyword);
    }

    public class AccountRepository : IAccountRepository
    {
        public Task<List<SystemAccount>> GetAccountsAsync() => AccountDAO.Instance.GetAccountsAsync();
        public Task<SystemAccount?> GetAccountByIdAsync(short accountId) => AccountDAO.Instance.GetAccountByIdAsync(accountId);
        public Task<SystemAccount?> GetAccountByEmailAsync(string email) => AccountDAO.Instance.GetAccountByEmailAsync(email);
        public Task<SystemAccount> AddAccountAsync(SystemAccount account) => AccountDAO.Instance.AddAccountAsync(account);
        public Task<bool> UpdateAccountAsync(SystemAccount account) => AccountDAO.Instance.UpdateAccountAsync(account);
        public Task<(bool Success, string Message)> DeleteAccountAsync(short accountId) => AccountDAO.Instance.DeleteAccountAsync(accountId);
        public Task<List<SystemAccount>> SearchAccountsAsync(string keyword) => AccountDAO.Instance.SearchAccountsAsync(keyword);
    }
}

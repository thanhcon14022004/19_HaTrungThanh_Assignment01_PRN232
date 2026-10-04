using _19_HaTrungThanh_Assignment01_BackEnd.DTOs;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using _19_HaTrungThanh_Assignment01_BackEnd.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public AccountsController(IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAccounts([FromQuery] string? keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchResult = await _accountRepository.SearchAccountsAsync(keyword);
                return Ok(searchResult);
            }
            var accounts = await _accountRepository.GetAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(short id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound(new { message = $"Account with ID {id} not found." });
            }
            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email conflicts with Admin email from appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            if (!string.IsNullOrEmpty(adminEmail) &&
                string.Equals(dto.AccountEmail?.Trim(), adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = $"Email '{dto.AccountEmail}' is reserved for Administrator." });
            }

            // Check if email already exists
            if (!string.IsNullOrEmpty(dto.AccountEmail))
            {
                var existingEmail = await _accountRepository.GetAccountByEmailAsync(dto.AccountEmail);
                if (existingEmail != null)
                {
                    return BadRequest(new { message = $"Email '{dto.AccountEmail}' already exists in system." });
                }
            }

            var account = new SystemAccount
            {
                AccountID = dto.AccountID,
                AccountName = dto.AccountName,
                AccountEmail = dto.AccountEmail,
                AccountRole = dto.AccountRole ?? 1,
                AccountPassword = dto.AccountPassword
            };

            var created = await _accountRepository.AddAccountAsync(account);
            return CreatedAtAction(nameof(GetAccountById), new { id = created.AccountID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(short id, [FromBody] AccountCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email conflicts with Admin email from appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            if (!string.IsNullOrEmpty(adminEmail) &&
                string.Equals(dto.AccountEmail?.Trim(), adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = $"Email '{dto.AccountEmail}' is reserved for Administrator." });
            }

            // Check if email already exists for another account
            if (!string.IsNullOrEmpty(dto.AccountEmail))
            {
                var existingEmail = await _accountRepository.GetAccountByEmailAsync(dto.AccountEmail);
                if (existingEmail != null && existingEmail.AccountID != id)
                {
                    return BadRequest(new { message = $"Email '{dto.AccountEmail}' already exists in system." });
                }
            }

            var account = new SystemAccount
            {
                AccountID = id,
                AccountName = dto.AccountName,
                AccountEmail = dto.AccountEmail,
                AccountRole = dto.AccountRole,
                AccountPassword = dto.AccountPassword
            };

            var updated = await _accountRepository.UpdateAccountAsync(account);
            if (!updated)
            {
                return NotFound(new { message = $"Account with ID {id} not found." });
            }

            return Ok(new { message = "Account updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(short id)
        {
            var result = await _accountRepository.DeleteAccountAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}

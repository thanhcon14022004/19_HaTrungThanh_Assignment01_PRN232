using _19_HaTrungThanh_Assignment01_BackEnd.DTOs;
using _19_HaTrungThanh_Assignment01_BackEnd.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Verify default Admin account from appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (!string.IsNullOrEmpty(adminEmail) &&
                string.Equals(request.Email.Trim(), adminEmail.Trim(), StringComparison.OrdinalIgnoreCase) &&
                request.Password == adminPassword)
            {
                return Ok(new LoginResponse
                {
                    AccountID = 0,
                    AccountName = "Administrator",
                    AccountEmail = adminEmail,
                    Role = "Admin",
                    RoleId = 0,
                    Message = "Admin login successful."
                });
            }

            // 2. Verify Member account from database via Repository
            var account = await _accountRepository.GetAccountByEmailAsync(request.Email);
            if (account == null || account.AccountPassword != request.Password)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Role: 1 = Staff, 2 = Lecturer
            string roleName = account.AccountRole switch
            {
                1 => "Staff",
                2 => "Lecturer",
                _ => "User"
            };

            return Ok(new LoginResponse
            {
                AccountID = account.AccountID,
                AccountName = account.AccountName ?? "User",
                AccountEmail = account.AccountEmail ?? string.Empty,
                Role = roleName,
                RoleId = account.AccountRole,
                Message = "Login successful."
            });
        }
    }
}

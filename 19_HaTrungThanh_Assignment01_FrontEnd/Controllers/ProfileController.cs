using _19_HaTrungThanh_Assignment01_FrontEnd.Models;
using _19_HaTrungThanh_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiClient _apiClient;

        public ProfileController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsStaff()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Staff";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsStaff()) return RedirectToAction("Index", "Login");

            short staffId = short.TryParse(HttpContext.Session.GetString("AccountID"), out var id) ? id : (short)0;
            var account = await _apiClient.GetAccountByIdAsync(staffId);
            if (account == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AccountViewModel model)
        {
            if (!IsStaff()) return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(model.AccountName) || string.IsNullOrWhiteSpace(model.AccountEmail))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ họ tên và email hợp lệ.";
                return RedirectToAction("Index");
            }

            short staffId = short.TryParse(HttpContext.Session.GetString("AccountID"), out var id) ? id : (short)0;
            model.AccountID = staffId;
            model.AccountRole = 1; // Staff role

            var (success, msg) = await _apiClient.UpdateAccountAsync(staffId, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật hồ sơ cá nhân thành công.";
                HttpContext.Session.SetString("AccountName", model.AccountName ?? "User");
                HttpContext.Session.SetString("AccountEmail", model.AccountEmail ?? "");
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }
    }
}

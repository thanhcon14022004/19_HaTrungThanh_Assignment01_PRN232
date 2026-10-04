using _19_HaTrungThanh_Assignment01_FrontEnd.Models;
using _19_HaTrungThanh_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _apiClient;

        public AccountController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            ViewBag.Keyword = keyword;
            var accounts = await _apiClient.GetAccountsAsync(keyword);
            return View(accounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ và chính xác thông tin tài khoản.";
                return RedirectToAction("Index");
            }

            var (success, msg) = await _apiClient.CreateAccountAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Tạo tài khoản mới thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, AccountViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            var (success, msg) = await _apiClient.UpdateAccountAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin tài khoản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(short id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            var (success, msg) = await _apiClient.DeleteAccountAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = msg.Contains("successfully") ? "Xóa tài khoản thành công." : msg;
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }
    }
}

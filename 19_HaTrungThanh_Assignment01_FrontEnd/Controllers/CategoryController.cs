using _19_HaTrungThanh_Assignment01_FrontEnd.Models;
using _19_HaTrungThanh_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApiClient _apiClient;

        public CategoryController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsAuthorized()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Staff" || role == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            ViewBag.Keyword = keyword;
            var categories = await _apiClient.GetCategoriesAsync(keyword);
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ và chính xác thông tin chuyên mục.";
                return RedirectToAction("Index");
            }

            var (success, msg) = await _apiClient.CreateCategoryAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Tạo chuyên mục mới thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, CategoryViewModel model)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            var (success, msg) = await _apiClient.UpdateCategoryAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật chuyên mục thành công.";
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
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            var (success, msg) = await _apiClient.DeleteCategoryAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = msg.Contains("successfully") ? "Xóa chuyên mục thành công." : msg;
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }
    }
}

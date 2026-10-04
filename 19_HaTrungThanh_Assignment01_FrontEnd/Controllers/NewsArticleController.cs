using _19_HaTrungThanh_Assignment01_FrontEnd.Models;
using _19_HaTrungThanh_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Controllers
{
    public class NewsArticleController : Controller
    {
        private readonly ApiClient _apiClient;

        public NewsArticleController(ApiClient apiClient)
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
            var articles = await _apiClient.GetNewsArticlesAsync(keyword);
            ViewBag.Categories = await _apiClient.GetCategoriesAsync();
            ViewBag.Tags = await _apiClient.GetTagsAsync();

            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            short staffId = short.TryParse(HttpContext.Session.GetString("AccountID"), out var id) ? id : (short)0;
            var history = await _apiClient.GetNewsHistoryAsync(staffId);
            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsArticleViewModel model)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            short staffId = short.TryParse(HttpContext.Session.GetString("AccountID"), out var id) ? id : (short)1;
            model.CreatedByID = staffId;
            model.UpdatedByID = staffId;

            if (string.IsNullOrWhiteSpace(model.NewsArticleID))
            {
                model.NewsArticleID = DateTime.Now.Ticks.ToString().Substring(10);
            }

            model.NewsStatus = Request.Form["NewsStatus"].Any(v => string.Equals(v, "true", StringComparison.OrdinalIgnoreCase));

            var (success, msg) = await _apiClient.CreateNewsArticleAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Đăng bài viết mới thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NewsArticleViewModel model)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            short staffId = short.TryParse(HttpContext.Session.GetString("AccountID"), out var idVal) ? idVal : (short)1;
            model.NewsArticleID = id;
            model.UpdatedByID = staffId;
            model.NewsStatus = Request.Form["NewsStatus"].Any(v => string.Equals(v, "true", StringComparison.OrdinalIgnoreCase));

            var (success, msg) = await _apiClient.UpdateNewsArticleAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật bài viết thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Login");

            var (success, msg) = await _apiClient.DeleteNewsArticleAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = msg.Contains("successfully") ? "Xóa bài viết thành công." : msg;
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction("Index");
        }
    }
}

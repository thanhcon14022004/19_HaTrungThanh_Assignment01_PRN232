using _19_HaTrungThanh_Assignment01_FrontEnd.Models;
using _19_HaTrungThanh_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApiClient _apiClient;

        public LoginController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin") return RedirectToAction("Index", "Account");
            if (role == "Staff") return RedirectToAction("Index", "NewsArticle");

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, data, error) = await _apiClient.LoginAsync(model);
            if (!success || data == null)
            {
                ViewBag.ErrorMessage = error;
                return View(model);
            }

            // Save user credentials into session
            HttpContext.Session.SetString("AccountID", data.AccountID.ToString());
            HttpContext.Session.SetString("AccountName", data.AccountName ?? "User");
            HttpContext.Session.SetString("AccountEmail", data.AccountEmail ?? "");
            HttpContext.Session.SetString("Role", data.Role ?? "");

            if (data.Role == "Admin")
            {
                return RedirectToAction("Index", "Account");
            }
            else if (data.Role == "Staff")
            {
                return RedirectToAction("Index", "NewsArticle");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

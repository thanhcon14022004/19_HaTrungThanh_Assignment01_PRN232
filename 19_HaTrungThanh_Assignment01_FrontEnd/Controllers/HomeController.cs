using _19_HaTrungThanh_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _apiClient;

        public HomeController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            ViewBag.Keyword = keyword;
            var activeNews = await _apiClient.GetActiveNewsArticlesAsync(keyword);
            return View(activeNews);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

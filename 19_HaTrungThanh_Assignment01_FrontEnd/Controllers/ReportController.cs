using _19_HaTrungThanh_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApiClient _apiClient;

        public ReportController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            if (!startDate.HasValue && !endDate.HasValue)
            {
                startDate = DateTime.Now.AddMonths(-12);
                endDate = DateTime.Now;
            }
            else if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                TempData["ErrorMessage"] = "Ngày bắt đầu không được lớn hơn ngày kết thúc. Hệ thống đã tự động đảo lại để hiển thị đúng khoảng thời gian.";
                (startDate, endDate) = (endDate, startDate);
            }

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            var report = await _apiClient.GetReportAsync(startDate, endDate);
            return View(report);
        }
    }
}

using _19_HaTrungThanh_Assignment01_BackEnd.DTOs;
using _19_HaTrungThanh_Assignment01_BackEnd.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public ReportsController(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.MinValue;
            DateTime end = endDate ?? DateTime.MaxValue;

            if (start > end)
            {
                (start, end) = (end, start);
            }

            var articles = await _newsArticleRepository.GetReportAsync(start, end);
            var report = new ReportStatisticDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalNews = articles.Count,
                Articles = articles
            };

            return Ok(report);
        }
    }
}

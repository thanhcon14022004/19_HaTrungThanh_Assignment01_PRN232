using _19_HaTrungThanh_Assignment01_BackEnd.DTOs;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using _19_HaTrungThanh_Assignment01_BackEnd.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public NewsArticlesController(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetNewsArticles([FromQuery] string? keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchResult = await _newsArticleRepository.SearchNewsArticlesAsync(keyword);
                return Ok(searchResult);
            }
            var articles = await _newsArticleRepository.GetNewsArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("active")]
        [EnableQuery]
        public async Task<IActionResult> GetActiveNewsArticles([FromQuery] string? keyword)
        {
            var articles = await _newsArticleRepository.GetActiveNewsArticlesAsync();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                articles = articles.Where(n => (n.NewsTitle != null && n.NewsTitle.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                               n.Headline.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                               (n.NewsContent != null && n.NewsContent.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                               (n.Category != null && n.Category.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                                   .ToList();
            }
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsArticleById(string id)
        {
            var article = await _newsArticleRepository.GetNewsArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound(new { message = $"News article with ID {id} not found." });
            }
            return Ok(article);
        }

        [HttpGet("history/{creatorId}")]
        public async Task<IActionResult> GetNewsHistory(short creatorId)
        {
            var history = await _newsArticleRepository.GetNewsArticlesByCreatorAsync(creatorId);
            return Ok(history);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewsArticle([FromBody] NewsArticleCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _newsArticleRepository.GetNewsArticleByIdAsync(dto.NewsArticleID);
            if (existing != null)
            {
                return BadRequest(new { message = $"News article ID '{dto.NewsArticleID}' already exists." });
            }

            var article = new NewsArticle
            {
                NewsArticleID = dto.NewsArticleID,
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryID = dto.CategoryID,
                NewsStatus = dto.NewsStatus ?? false,
                CreatedByID = dto.CreatedByID,
                UpdatedByID = dto.CreatedByID,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            var created = await _newsArticleRepository.AddNewsArticleAsync(article, dto.TagIds);
            return CreatedAtAction(nameof(GetNewsArticleById), new { id = created.NewsArticleID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNewsArticle(string id, [FromBody] NewsArticleCreateUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewsArticleID))
            {
                dto.NewsArticleID = id;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = new NewsArticle
            {
                NewsArticleID = id,
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryID = dto.CategoryID,
                NewsStatus = dto.NewsStatus,
                UpdatedByID = dto.UpdatedByID,
                ModifiedDate = DateTime.Now
            };

            var updated = await _newsArticleRepository.UpdateNewsArticleAsync(article, dto.TagIds);
            if (!updated)
            {
                return NotFound(new { message = $"News article with ID {id} not found." });
            }

            return Ok(new { message = "News article updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsArticle(string id)
        {
            var result = await _newsArticleRepository.DeleteNewsArticleAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}

using _19_HaTrungThanh_Assignment01_BackEnd.DTOs;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using _19_HaTrungThanh_Assignment01_BackEnd.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetCategories([FromQuery] string? keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchResult = await _categoryRepository.SearchCategoriesAsync(keyword);
                return Ok(searchResult);
            }
            var categories = await _categoryRepository.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(short id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                CategoryDesciption = dto.CategoryDesciption,
                ParentCategoryID = dto.ParentCategoryID,
                IsActive = dto.IsActive ?? true
            };

            var created = await _categoryRepository.AddCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = created.CategoryID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(short id, [FromBody] CategoryCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.ParentCategoryID.HasValue && dto.ParentCategoryID.Value == id)
            {
                return BadRequest(new { message = "A category cannot be set as its own parent category." });
            }

            var category = new Category
            {
                CategoryID = id,
                CategoryName = dto.CategoryName,
                CategoryDesciption = dto.CategoryDesciption,
                ParentCategoryID = dto.ParentCategoryID,
                IsActive = dto.IsActive
            };

            var updated = await _categoryRepository.UpdateCategoryAsync(category);
            if (!updated)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            return Ok(new { message = "Category updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            var result = await _categoryRepository.DeleteCategoryAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}

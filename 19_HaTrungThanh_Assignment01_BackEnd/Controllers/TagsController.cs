using _19_HaTrungThanh_Assignment01_BackEnd.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.GetTagsAsync();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var tag = await _tagRepository.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound(new { message = $"Tag with ID {id} not found." });
            }
            return Ok(tag);
        }
    }
}

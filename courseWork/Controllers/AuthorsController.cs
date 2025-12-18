using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorRequest request)
        {
            var createdAuthor = await _authorService.CreateAuthorAsync(request);
            return Ok(createdAuthor);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] CreateAuthorRequest request)
        {
            try
            {
                var updatedAuthor = await _authorService.UpdateAuthorAsync(id, request);
                return Ok(updatedAuthor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                await _authorService.DeleteAuthorAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}

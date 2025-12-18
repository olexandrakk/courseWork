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

    }
}

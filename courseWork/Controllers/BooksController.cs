using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("get-all-books")]
        public async Task<IActionResult> GetAllBooks(GetBooksRequest request)
        {
            var books = await _bookService.GetAllBooksAsync(request);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
        {
            try
            {
                var createdBook = await _bookService.CreateBookAsync(request);
                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.BookID }, createdBook);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] CreateBookRequest request)
        {
            try
            {
                var updatedBook = await _bookService.UpdateBookAsync(id, request);
                return Ok(updatedBook);
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
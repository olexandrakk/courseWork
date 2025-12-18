using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookStoresController : ControllerBase
    {
        private readonly IBookStoreService _bookStoreService;

        public BookStoresController(IBookStoreService bookStoreService)
        {
            _bookStoreService = bookStoreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stores = await _bookStoreService.GetAllStoresAsync();
            return Ok(stores);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookStoreRequest request)
        {
            var createdStore = await _bookStoreService.CreateBookStoreAsync(request);
            return Ok(createdStore);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBookStoreRequest request)
        {
            try
            {
                var updatedStore = await _bookStoreService.UpdateBookStoreAsync(id, request);
                return Ok(updatedStore);
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _bookStoreService.DeleteBookStoreAsync(id);
                return NoContent();
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
    }
}
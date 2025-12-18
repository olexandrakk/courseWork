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
    }
}
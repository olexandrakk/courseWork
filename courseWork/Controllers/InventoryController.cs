using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Отримати інвентар конкретного магазину: GET api/inventory/store/1
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetByStore(int storeId)
        {
            var items = await _inventoryService.GetStoreInventoryAsync(storeId);
            return Ok(items);
        }

        // Оновити кількість товару
        [HttpPost("update-stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockRequest request)
        {
            try
            {
                await _inventoryService.UpdateStockAsync(request.BookId, request.BookStoreId, request.NewQuantity);
                return Ok(new { message = "Stock updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // Допоміжний клас для запиту (можна винести в Requests)
    public class UpdateStockRequest
    {
        public int BookId { get; set; }
        public int BookStoreId { get; set; }
        public int NewQuantity { get; set; }
    }
}
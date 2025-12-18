using courseWork.BLL.Common.Requests.Inventory;
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

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetByStore(int storeId)
        {
            var items = await _inventoryService.GetStoreInventoryAsync(storeId);
            return Ok(items);
        }

        [HttpPost("update-stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockRequest request)
        {
            try
            {
                await _inventoryService.UpdateStockAsync(request);
                return Ok(new { message = "Stock updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("book/{bookId}/store/{bookStoreId}")]
        public async Task<IActionResult> DeleteInventory(int bookId, int bookStoreId)
        {
            try
            {
                await _inventoryService.DeleteInventoryAsync(bookId, bookStoreId);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
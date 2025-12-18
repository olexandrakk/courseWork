using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(request);
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing the order." });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpPut("{orderNumber}")]
        public async Task<IActionResult> UpdateOrder(int orderNumber, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrderAsync(orderNumber, request);
                return Ok(updatedOrder);
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{orderNumber}")]
        public async Task<IActionResult> DeleteOrder(int orderNumber)
        {
            try
            {
                await _orderService.DeleteOrderAsync(orderNumber);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
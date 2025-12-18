using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
        Task<List<OrderDto>> GetUserOrdersAsync(int userId);
        Task<OrderDto> UpdateOrderAsync(int orderNumber, UpdateOrderRequest request);
        Task DeleteOrderAsync(int orderNumber);
    }
}
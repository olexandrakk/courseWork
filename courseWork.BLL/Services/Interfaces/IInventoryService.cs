using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests.Inventory;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDto> CreateInventoryAsync(CreateInventoryRequest request);
        Task<List<InventoryDto>> GetAllInventoryAsync();
        Task<List<InventoryDto>> GetStoreInventoryAsync(int storeId);
        Task<InventoryDto> UpdateStockAsync(UpdateStockRequest request);
    }
}

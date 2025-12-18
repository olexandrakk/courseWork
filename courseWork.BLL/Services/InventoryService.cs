using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests.Inventory;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<BookStore> _storeRepository;
        private readonly IMapper _mapper;

        public InventoryService(
            IRepository<Inventory> inventoryRepository,
            IRepository<BookStore> storeRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public Task<InventoryDto> CreateInventoryAsync(CreateInventoryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<InventoryDto>> GetAllInventoryAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<BookStoreDto>> GetAllStoresAsync()
        {
            var stores = await _storeRepository.ToListAsync();
            return _mapper.Map<List<BookStoreDto>>(stores);
        }

        public async Task<List<InventoryDto>> GetStoreInventoryAsync(int storeId)
        {
            var inventory = await _inventoryRepository
                .Include(i => i.Book)      
                .Include(i => i.BookStore)  
                .Where(i => i.BookStoreID == storeId)
                .ToListAsync();

            return _mapper.Map<List<InventoryDto>>(inventory);
        }

        public async Task<InventoryDto> UpdateStockAsync(UpdateStockRequest request)
        {
            var inventory = await _inventoryRepository.FirstOrDefaultAsync(i => i.BookID == request.BookId && i.BookStoreID == request.BookStoreId);

            if (inventory != null)
            {
                _mapper.Map(request, inventory);
                await _inventoryRepository.UpdateAsync(inventory);
            }
            else
            {
                inventory = _mapper.Map<Inventory>(request);
                await _inventoryRepository.InsertAsync(inventory);
            }

            return _mapper.Map<InventoryDto>(inventory);
        }
    }
}

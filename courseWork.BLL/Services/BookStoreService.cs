using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class BookStoreService : IBookStoreService
    {
        private readonly IRepository<BookStore> _storeRepository;
        private readonly IMapper _mapper;

        public BookStoreService(IRepository<BookStore> storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<List<BookStoreDto>> GetAllStoresAsync()
        {
            var stores = await _storeRepository.ToListAsync(); 
            return _mapper.Map<List<BookStoreDto>>(stores);
        }

        public async Task<BookStoreDto> CreateBookStoreAsync(CreateBookStoreRequest request)
        {
            var store = _mapper.Map<BookStore>(request);
            await _storeRepository.InsertAsync(store);
            return _mapper.Map<BookStoreDto>(store);
        }

        public async Task<BookStoreDto> UpdateBookStoreAsync(int id, CreateBookStoreRequest request)
        {
            var store = await _storeRepository
                .FirstOrDefaultAsync(s => s.BookStoreID == id);

            if (store == null)
                throw new Exception($"BookStore with current Id doesn't exist: {id}");

            store.Name = request.Name;
            store.Address = request.Address;

            await _storeRepository.UpdateAsync(store);

            return _mapper.Map<BookStoreDto>(store);
        }

        public async Task DeleteBookStoreAsync(int id)
        {
            var store = await _storeRepository
                .Include(s => s.Inventories)
                .FirstOrDefaultAsync(s => s.BookStoreID == id);

            if (store == null)
                throw new Exception($"BookStore with current Id doesn't exist: {id}");

            if (store.Inventories.Any())
                throw new InvalidOperationException("Cannot delete book store with associated inventory.");

            await _storeRepository.DeleteAsync(store);
        }
    }
}
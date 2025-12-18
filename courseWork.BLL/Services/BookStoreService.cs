using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.DBContext;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class BookStoreService : IBookStoreService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<BookStore> _storeRepository;
        private readonly IMapper _mapper;

        public BookStoreService(ApplicationDbContext context, IRepository<BookStore> storeRepository, IMapper mapper)
        {
            _context = context;
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
    }
}
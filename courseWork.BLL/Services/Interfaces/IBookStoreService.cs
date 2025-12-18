using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IBookStoreService
    {
        Task<BookStoreDto> CreateBookStoreAsync(CreateBookStoreRequest request);
        Task<List<BookStoreDto>> GetAllStoresAsync();
    }
}

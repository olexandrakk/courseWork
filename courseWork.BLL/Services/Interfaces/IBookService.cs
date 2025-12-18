using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Common.Requests.Pagination;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IBookService
    {
        Task<PagedResult<BookDto>> GetAllBooksAsync(GetBooksRequest request);
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookRequest request);
        Task<BookDto> UpdateBookAsync(int id, CreateBookRequest request);
        Task DeleteBookAsync(int id);
    }
}

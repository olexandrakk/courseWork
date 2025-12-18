using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookRequest request);
    }
}

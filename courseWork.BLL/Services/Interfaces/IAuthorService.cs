using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAuthorsAsync();
        Task<AuthorDto> CreateAuthorAsync(CreateAuthorRequest request);
        Task<AuthorDto> UpdateAuthorAsync(int authorId, CreateAuthorRequest request);
        Task DeleteAuthorAsync(int authorId);
    }
}

using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Common.Requests.Pagination;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(CreateUserRequest request);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<PagedResult<UserDto>> GetAllUsersAsync(GetUsersRequest request);
        Task<bool> DeleteUserAsync(int id);
    }
}

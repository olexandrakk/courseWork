using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(CreateUserRequest request);

        Task<UserDto> GetUserByIdAsync(int id);
        Task<bool> DeleteUserAsync(int id);
    }
}

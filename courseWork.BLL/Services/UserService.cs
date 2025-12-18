using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Common.Requests.Pagination;
using courseWork.BLL.Extensions;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;


namespace courseWork.BLL.Services
{

    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserID == id && !x.IsDeleted);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<PagedResult<UserDto>> GetAllUsersAsync(GetUsersRequest request)
        {
            var query = _userRepository
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(u => u.Email.Contains(request.Email));
            }

            var pagedUsers = await query
                .ToPagedResultAsync(request.Page, request.PageSize);

            return new PagedResult<UserDto>
            {
                Items = _mapper.Map<List<UserDto>>(pagedUsers.Items),
                Page = pagedUsers.Page,
                PageSize = pagedUsers.PageSize,
                TotalCount = pagedUsers.TotalCount
            };
        }

        public async Task<UserDto> RegisterUserAsync(CreateUserRequest request)
        {
            var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = _mapper.Map<User>(request);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _userRepository.InsertAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserID == id);

            if (user == null)
                throw new Exception($"User with current Id doesn't exist: {id}");

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}

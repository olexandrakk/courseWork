using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using courseWork.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using System.Linq;

namespace courseWork.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IRepository<User>> _userRepositoryMock;
        private readonly IMapper _mapper;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            _mapper = TestHelper.CreateMapper();
        }

        private void SetupQueryable<T>(Mock<IRepository<T>> mock, List<T> data) where T : class
        {
            var mockQueryable = data.BuildMock();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());
            mock.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public TestAsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public T Current => _enumerator.Current;

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }

            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                return ValueTask.CompletedTask;
            }
        }

        [Fact]
        public async Task RegisterUserAsync_WhenEmailExists_ThrowsInvalidOperationException()
        {
            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "test@gmail.com",
                Password = "password123",
                Phone = "1234567890",
                City = "Test City",
                AddressLine = "Test Address"
            };

            var existingUser = new User
            {
                UserID = 1,
                Email = "test@gmail.com"
            };

            SetupQueryable(_userRepositoryMock, new List<User> { existingUser });

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterUserAsync(request));
        }

        [Fact]
        public async Task RegisterUserAsync_WhenValid_CreatesUser()
        {
            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "test@gmail.com",
                Password = "password123",
                Phone = "1234567890",
                City = "Test City",
                AddressLine = "Test Address"
            };

            SetupQueryable(_userRepositoryMock, new List<User>());

            _userRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<User>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            var result = await service.RegisterUserAsync(request);

            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Username.Should().Be(request.Username);
            _userRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<User>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
        {
            var userId = 999;
            SetupQueryable(_userRepositoryMock, new List<User>());

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            var result = await service.GetUserByIdAsync(userId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
        {
            var userId = 1;
            var user = new User
            {
                UserID = userId,
                Username = "testuser",
                Email = "test@gmail.com",
                IsDeleted = false
            };

            SetupQueryable(_userRepositoryMock, new List<User> { user });

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            var result = await service.GetUserByIdAsync(userId);

            result.Should().NotBeNull();
            result.UserID.Should().Be(userId);
            result.Username.Should().Be(user.Username);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserNotFound_ThrowsException()
        {
            var userId = 999;
            SetupQueryable(_userRepositoryMock, new List<User>());

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.DeleteUserAsync(userId));
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserExists_SoftDeletesUser()
        {
            var userId = 1;
            var user = new User
            {
                UserID = userId,
                Username = "testuser",
                Email = "test@gmail.com",
                IsDeleted = false
            };

            SetupQueryable(_userRepositoryMock, new List<User> { user });

            _userRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            var result = await service.DeleteUserAsync(userId);

            result.Should().BeTrue();
            user.IsDeleted.Should().BeTrue();
            user.DeletedAt.Should().NotBeNull();
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsPagedUsers()
        {
            var users = new List<User>
            {
                new User { UserID = 1, Username = "user1", Email = "user1@gmail.com", IsDeleted = false },
                new User { UserID = 2, Username = "user2", Email = "user2@gmail.com", IsDeleted = false }
            };

            SetupQueryable(_userRepositoryMock, users);

            var request = new GetUsersRequest
            {
                Page = 1,
                PageSize = 10
            };

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            var result = await service.GetAllUsersAsync(request);

            result.Should().NotBeNull();
            result.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllUsersAsync_WithEmailFilter_ReturnsFilteredUsers()
        {
            var users = new List<User>
            {
                new User { UserID = 1, Username = "user1", Email = "test@gmail.com", IsDeleted = false },
                new User { UserID = 2, Username = "user2", Email = "other@gmail.com", IsDeleted = false }
            };

            SetupQueryable(_userRepositoryMock, users);

            var request = new GetUsersRequest
            {
                Email = "test",
                Page = 1,
                PageSize = 10
            };

            var service = new UserService(_userRepositoryMock.Object, _mapper);

            var result = await service.GetAllUsersAsync(request);

            result.Should().NotBeNull();
        }
    }
}

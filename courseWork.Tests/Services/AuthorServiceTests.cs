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
    public class AuthorServiceTests
    {
        private readonly Mock<IRepository<Author>> _authorRepositoryMock;
        private readonly IMapper _mapper;

        public AuthorServiceTests()
        {
            _authorRepositoryMock = new Mock<IRepository<Author>>();
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
        public async Task CreateAuthorAsync_WhenNameExists_ThrowsInvalidOperationException()
        {
            var request = new CreateAuthorRequest
            {
                Name = "Test Author",
                Email = "test@gmail.com"
            };

            var existingAuthor = new Author
            {
                AuthorID = 1,
                Name = "Test Author",
                Email = "existing@gmail.com"
            };

            SetupQueryable(_authorRepositoryMock, new List<Author> { existingAuthor });

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateAuthorAsync(request));
        }

        [Fact]
        public async Task CreateAuthorAsync_WhenValid_CreatesAuthor()
        {
            var request = new CreateAuthorRequest
            {
                Name = "New Author",
                Email = "new@gmail.com"
            };

            SetupQueryable(_authorRepositoryMock, new List<Author>());

            _authorRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Author>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            var result = await service.CreateAuthorAsync(request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            result.Email.Should().Be(request.Email);
            _authorRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Author>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ReturnsAllAuthors()
        {
            var authors = new List<Author>
            {
                new Author { AuthorID = 1, Name = "Author 1", Email = "author1@gmail.com" },
                new Author { AuthorID = 2, Name = "Author 2", Email = "author2@gmail.com" }
            };

            SetupQueryable(_authorRepositoryMock, authors);

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            var result = await service.GetAllAuthorsAsync();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task UpdateAuthorAsync_WhenAuthorNotFound_ThrowsKeyNotFoundException()
        {
            var authorId = 999;
            var request = new CreateAuthorRequest
            {
                Name = "Updated Author",
                Email = "updated@gmail.com"
            };

            SetupQueryable(_authorRepositoryMock, new List<Author>());

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.UpdateAuthorAsync(authorId, request));
        }

        [Fact]
        public async Task UpdateAuthorAsync_WhenNameExists_ThrowsInvalidOperationException()
        {
            var authorId = 1;
            var existingAuthor = new Author
            {
                AuthorID = authorId,
                Name = "Original Author",
                Email = "original@gmail.com"
            };

            var anotherAuthor = new Author
            {
                AuthorID = 2,
                Name = "Another Author",
                Email = "another@gmail.com"
            };

            var request = new CreateAuthorRequest
            {
                Name = "Another Author",
                Email = "updated@gmail.com"
            };

            SetupQueryable(_authorRepositoryMock, new List<Author> { existingAuthor, anotherAuthor });

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateAuthorAsync(authorId, request));
        }

        [Fact]
        public async Task UpdateAuthorAsync_WhenValid_UpdatesAuthor()
        {
            var authorId = 1;
            var author = new Author
            {
                AuthorID = authorId,
                Name = "Original Author",
                Email = "original@gmail.com"
            };

            var request = new CreateAuthorRequest
            {
                Name = "Updated Author",
                Email = "updated@gmail.com"
            };

            SetupQueryable(_authorRepositoryMock, new List<Author> { author });

            _authorRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            var result = await service.UpdateAuthorAsync(authorId, request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            _authorRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthorAsync_WhenAuthorNotFound_ThrowsKeyNotFoundException()
        {
            var authorId = 999;
            SetupQueryable(_authorRepositoryMock, new List<Author>());

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.DeleteAuthorAsync(authorId));
        }

        [Fact]
        public async Task DeleteAuthorAsync_WhenAuthorExists_DeletesAuthor()
        {
            var authorId = 1;
            var author = new Author
            {
                AuthorID = authorId,
                Name = "Test Author",
                Email = "test@gmail.com",
                Books = new List<Book>()
            };

            SetupQueryable(_authorRepositoryMock, new List<Author> { author });

            _authorRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Author>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new AuthorService(_authorRepositoryMock.Object, _mapper);

            await service.DeleteAuthorAsync(authorId);

            _authorRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Author>(), It.IsAny<bool>()), Times.Once);
        }
    }
}


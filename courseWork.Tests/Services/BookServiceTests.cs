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
    public class BookServiceTests
    {
        private readonly Mock<IRepository<Book>> _bookRepositoryMock;
        private readonly Mock<IRepository<Author>> _authorRepositoryMock;
        private readonly IMapper _mapper;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IRepository<Book>>();
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
        public async Task CreateBookAsync_WhenISBNExists_ThrowsInvalidOperationException()
        {
            var request = new CreateBookRequest
            {
                ISBN = "1234567890",
                Name = "Test Book",
                NumberOfPages = 100,
                Price = 29.99m,
                PublisherID = 1,
                AuthorIds = new List<int> { 1 }
            };

            var existingBook = new Book { BookID = 1, ISBN = "1234567890" };
            SetupQueryable(_bookRepositoryMock, new List<Book> { existingBook });

            var service = new BookService(_bookRepositoryMock.Object, _mapper, _authorRepositoryMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateBookAsync(request));
        }

        [Fact]
        public async Task CreateBookAsync_WhenAuthorNotFound_ThrowsInvalidOperationException()
        {
            var request = new CreateBookRequest
            {
                ISBN = "1234567890",
                Name = "Test Book",
                NumberOfPages = 100,
                Price = 29.99m,
                PublisherID = 1,
                AuthorIds = new List<int> { 1, 2 }
            };

            SetupQueryable(_bookRepositoryMock, new List<Book>());
            var authors = new List<Author> { new Author { AuthorID = 1 } };
            SetupQueryable(_authorRepositoryMock, authors);

            var service = new BookService(_bookRepositoryMock.Object, _mapper, _authorRepositoryMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateBookAsync(request));
        }

        [Fact]
        public async Task DeleteBookAsync_WhenBookNotFound_ThrowsException()
        {
            var bookId = 999;
            SetupQueryable(_bookRepositoryMock, new List<Book>());

            var service = new BookService(_bookRepositoryMock.Object, _mapper, _authorRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(
                () => service.DeleteBookAsync(bookId));
        }

        [Fact]
        public async Task DeleteBookAsync_WhenBookExists_SoftDeletesBook()
        {
            var bookId = 1;
            var book = new Book
            {
                BookID = bookId,
                ISBN = "1234567890",
                Name = "Test Book",
                IsDeleted = false
            };

            SetupQueryable(_bookRepositoryMock, new List<Book> { book });

            _bookRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new BookService(_bookRepositoryMock.Object, _mapper, _authorRepositoryMock.Object);

            await service.DeleteBookAsync(bookId);

            book.IsDeleted.Should().BeTrue();
            book.DeletedAt.Should().NotBeNull();
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_WhenBookNotFound_ThrowsException()
        {
            var bookId = 999;
            var request = new CreateBookRequest
            {
                ISBN = "1234567890",
                Name = "Updated Book",
                NumberOfPages = 200,
                Price = 39.99m,
                PublisherID = 1,
                AuthorIds = new List<int> { 1 }
            };

            SetupQueryable(_bookRepositoryMock, new List<Book>());

            var service = new BookService(_bookRepositoryMock.Object, _mapper, _authorRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(
                () => service.UpdateBookAsync(bookId, request));
        }

        [Fact]
        public async Task GetBookByIdAsync_WhenBookNotFound_ThrowsException()
        {
            var bookId = 999;
            SetupQueryable(_bookRepositoryMock, new List<Book>());

            var service = new BookService(_bookRepositoryMock.Object, _mapper, _authorRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(
                () => service.GetBookByIdAsync(bookId));
        }
    }
}

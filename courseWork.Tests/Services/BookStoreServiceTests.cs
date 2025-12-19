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
    public class BookStoreServiceTests
    {
        private readonly Mock<IRepository<BookStore>> _storeRepositoryMock;
        private readonly IMapper _mapper;

        public BookStoreServiceTests()
        {
            _storeRepositoryMock = new Mock<IRepository<BookStore>>();
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
        public async Task CreateBookStoreAsync_WhenValid_CreatesBookStore()
        {
            var request = new CreateBookStoreRequest
            {
                Name = "New Book Store",
                Address = "New Address",
                City = "New City"
            };

            _storeRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<BookStore>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new BookStoreService(_storeRepositoryMock.Object, _mapper);

            var result = await service.CreateBookStoreAsync(request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            result.Address.Should().Be(request.Address);
            _storeRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<BookStore>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetAllStoresAsync_ReturnsAllStores()
        {
            var stores = new List<BookStore>
            {
                new BookStore { BookStoreID = 1, Name = "Store 1", Address = "Address 1" },
                new BookStore { BookStoreID = 2, Name = "Store 2", Address = "Address 2" }
            };

            SetupQueryable(_storeRepositoryMock, stores);

            var service = new BookStoreService(_storeRepositoryMock.Object, _mapper);

            var result = await service.GetAllStoresAsync();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task UpdateBookStoreAsync_WhenStoreNotFound_ThrowsException()
        {
            var storeId = 999;
            var request = new CreateBookStoreRequest
            {
                Name = "Updated Store",
                Address = "Updated Address",
                City = "Updated City"
            };

            SetupQueryable(_storeRepositoryMock, new List<BookStore>());

            var service = new BookStoreService(_storeRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.UpdateBookStoreAsync(storeId, request));
        }

        [Fact]
        public async Task UpdateBookStoreAsync_WhenValid_UpdatesStore()
        {
            var storeId = 1;
            var store = new BookStore
            {
                BookStoreID = storeId,
                Name = "Original Store",
                Address = "Original Address"
            };

            var request = new CreateBookStoreRequest
            {
                Name = "Updated Store",
                Address = "Updated Address",
                City = "Updated City"
            };

            SetupQueryable(_storeRepositoryMock, new List<BookStore> { store });

            _storeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<BookStore>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new BookStoreService(_storeRepositoryMock.Object, _mapper);

            var result = await service.UpdateBookStoreAsync(storeId, request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            result.Address.Should().Be(request.Address);
            _storeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<BookStore>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBookStoreAsync_WhenStoreNotFound_ThrowsException()
        {
            var storeId = 999;
            SetupQueryable(_storeRepositoryMock, new List<BookStore>());

            var service = new BookStoreService(_storeRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.DeleteBookStoreAsync(storeId));
        }

        [Fact]
        public async Task DeleteBookStoreAsync_WhenStoreHasInventory_ThrowsInvalidOperationException()
        {
            var storeId = 1;
            var store = new BookStore
            {
                BookStoreID = storeId,
                Name = "Test Store",
                Address = "Test Address",
                Inventories = new List<Inventory> { new Inventory { InventoryID = 1 } }
            };

            SetupQueryable(_storeRepositoryMock, new List<BookStore> { store });

            var service = new BookStoreService(_storeRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.DeleteBookStoreAsync(storeId));
        }

        [Fact]
        public async Task DeleteBookStoreAsync_WhenValid_DeletesStore()
        {
            var storeId = 1;
            var store = new BookStore
            {
                BookStoreID = storeId,
                Name = "Test Store",
                Address = "Test Address",
                Inventories = new List<Inventory>()
            };

            SetupQueryable(_storeRepositoryMock, new List<BookStore> { store });

            _storeRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<BookStore>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new BookStoreService(_storeRepositoryMock.Object, _mapper);

            await service.DeleteBookStoreAsync(storeId);

            _storeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<BookStore>(), It.IsAny<bool>()), Times.Once);
        }
    }
}


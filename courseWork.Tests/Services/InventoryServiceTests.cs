using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests.Inventory;
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
    public class InventoryServiceTests
    {
        private readonly Mock<IRepository<Inventory>> _inventoryRepositoryMock;
        private readonly Mock<IRepository<BookStore>> _storeRepositoryMock;
        private readonly IMapper _mapper;

        public InventoryServiceTests()
        {
            _inventoryRepositoryMock = new Mock<IRepository<Inventory>>();
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
        public async Task GetStoreInventoryAsync_ReturnsInventoryForStore()
        {
            var storeId = 1;
            var inventory = new List<Inventory>
            {
                new Inventory
                {
                    InventoryID = 1,
                    BookStoreID = storeId,
                    BookID = 1,
                    StockQuantity = 10,
                    Book = new Book { BookID = 1, Name = "Book 1" },
                    BookStore = new BookStore { BookStoreID = storeId, Name = "Store 1" }
                },
                new Inventory
                {
                    InventoryID = 2,
                    BookStoreID = storeId,
                    BookID = 2,
                    StockQuantity = 5,
                    Book = new Book { BookID = 2, Name = "Book 2" },
                    BookStore = new BookStore { BookStoreID = storeId, Name = "Store 1" }
                }
            };

            SetupQueryable(_inventoryRepositoryMock, inventory);

            var service = new InventoryService(_inventoryRepositoryMock.Object, _storeRepositoryMock.Object, _mapper);

            var result = await service.GetStoreInventoryAsync(storeId);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task UpdateStockAsync_WhenInventoryExists_UpdatesStock()
        {
            var request = new UpdateStockRequest
            {
                BookId = 1,
                BookStoreId = 1,
                NewQuantity = 20
            };

            var existingInventory = new Inventory
            {
                InventoryID = 1,
                BookID = request.BookId,
                BookStoreID = request.BookStoreId,
                StockQuantity = 10
            };

            SetupQueryable(_inventoryRepositoryMock, new List<Inventory> { existingInventory });

            _inventoryRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Inventory>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new InventoryService(_inventoryRepositoryMock.Object, _storeRepositoryMock.Object, _mapper);

            var result = await service.UpdateStockAsync(request);

            result.Should().NotBeNull();
            _inventoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Inventory>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStockAsync_WhenInventoryNotExists_CreatesInventory()
        {
            var request = new UpdateStockRequest
            {
                BookId = 1,
                BookStoreId = 1,
                NewQuantity = 20
            };

            SetupQueryable(_inventoryRepositoryMock, new List<Inventory>());

            _inventoryRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Inventory>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new InventoryService(_inventoryRepositoryMock.Object, _storeRepositoryMock.Object, _mapper);

            var result = await service.UpdateStockAsync(request);

            result.Should().NotBeNull();
            _inventoryRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Inventory>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task DeleteInventoryAsync_WhenInventoryNotFound_ThrowsException()
        {
            var bookId = 999;
            var bookStoreId = 999;

            SetupQueryable(_inventoryRepositoryMock, new List<Inventory>());

            var service = new InventoryService(_inventoryRepositoryMock.Object, _storeRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.DeleteInventoryAsync(bookId, bookStoreId));
        }

        [Fact]
        public async Task DeleteInventoryAsync_WhenInventoryExists_DeletesInventory()
        {
            var bookId = 1;
            var bookStoreId = 1;
            var inventory = new Inventory
            {
                InventoryID = 1,
                BookID = bookId,
                BookStoreID = bookStoreId,
                StockQuantity = 10
            };

            SetupQueryable(_inventoryRepositoryMock, new List<Inventory> { inventory });

            _inventoryRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Inventory>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new InventoryService(_inventoryRepositoryMock.Object, _storeRepositoryMock.Object, _mapper);

            await service.DeleteInventoryAsync(bookId, bookStoreId);

            _inventoryRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Inventory>(), It.IsAny<bool>()), Times.Once);
        }
    }
}


using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services;
using courseWork.DAL.DBContext;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using courseWork.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using System.Linq;

namespace courseWork.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IRepository<OrderDetails>> _orderDetailsRepositoryMock;
        private readonly Mock<IRepository<Inventory>> _inventoryRepositoryMock;
        private readonly Mock<IRepository<Book>> _bookRepositoryMock;
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly IMapper _mapper;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IRepository<Order>>();
            _orderDetailsRepositoryMock = new Mock<IRepository<OrderDetails>>();
            _inventoryRepositoryMock = new Mock<IRepository<Inventory>>();
            _bookRepositoryMock = new Mock<IRepository<Book>>();
            _contextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _transactionMock = new Mock<IDbContextTransaction>();
            _mapper = TestHelper.CreateMapper();
        }

        private void SetupQueryable<T>(Mock<IRepository<T>> mock, List<T> data) where T : class
        {
            var mockQueryable = data.BuildMock();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());
        }

        [Fact]
        public async Task CreateOrderAsync_WhenInsufficientStock_ThrowsInvalidOperationException()
        {
            var request = new CreateOrderRequest
            {
                UserID = 1,
                BookStoreID = 1,
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { BookId = 1, Quantity = 100 }
                }
            };

            var inventory = new Inventory
            {
                InventoryID = 1,
                BookID = 1,
                BookStoreID = 1,
                StockQuantity = 10
            };

            SetupQueryable(_inventoryRepositoryMock, new List<Inventory> { inventory });

            _contextMock
                .Setup(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            var service = new OrderService(
                _orderRepositoryMock.Object,
                _orderDetailsRepositoryMock.Object,
                _inventoryRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _contextMock.Object,
                _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateOrderAsync(request));
        }

        [Fact]
        public async Task UpdateOrderAsync_WhenOrderNotFound_ThrowsException()
        {
            var orderNumber = 999;
            var request = new UpdateOrderRequest
            {
                Status = "Completed"
            };

            SetupQueryable(_orderRepositoryMock, new List<Order>());

            var service = new OrderService(
                _orderRepositoryMock.Object,
                _orderDetailsRepositoryMock.Object,
                _inventoryRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _contextMock.Object,
                _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.UpdateOrderAsync(orderNumber, request));
        }

        [Fact]
        public async Task UpdateOrderAsync_WhenValid_UpdatesOrder()
        {
            var orderNumber = 1;
            var order = new Order
            {
                OrderNumber = orderNumber,
                UserID = 1,
                Status = "Pending",
                Price = 100m,
                UpdateAt = DateTime.UtcNow.AddDays(-1)
            };

            var request = new UpdateOrderRequest
            {
                Status = "Completed"
            };

            SetupQueryable(_orderRepositoryMock, new List<Order> { order });

            _orderRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new OrderService(
                _orderRepositoryMock.Object,
                _orderDetailsRepositoryMock.Object,
                _inventoryRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _contextMock.Object,
                _mapper);

            var result = await service.UpdateOrderAsync(orderNumber, request);

            result.Should().NotBeNull();
            order.Status.Should().Be(request.Status);
            _orderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderNotFound_ThrowsException()
        {
            var orderNumber = 999;
            SetupQueryable(_orderRepositoryMock, new List<Order>());

            var service = new OrderService(
                _orderRepositoryMock.Object,
                _orderDetailsRepositoryMock.Object,
                _inventoryRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _contextMock.Object,
                _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.DeleteOrderAsync(orderNumber));
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderExists_SoftDeletesOrder()
        {
            var orderNumber = 1;
            var order = new Order
            {
                OrderNumber = orderNumber,
                UserID = 1,
                Status = "Pending",
                Price = 100m,
                IsDeleted = false
            };

            SetupQueryable(_orderRepositoryMock, new List<Order> { order });

            _orderRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new OrderService(
                _orderRepositoryMock.Object,
                _orderDetailsRepositoryMock.Object,
                _inventoryRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _contextMock.Object,
                _mapper);

            await service.DeleteOrderAsync(orderNumber);

            order.IsDeleted.Should().BeTrue();
            order.DeletedAt.Should().NotBeNull();
            _orderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<bool>()), Times.Once);
        }
    }
}


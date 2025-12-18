using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.DBContext;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetails> _orderDetailsRepository; 
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<OrderDetails> orderDetailsRepository,
            IRepository<Inventory> inventoryRepository,
            IRepository<Book> bookRepository,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _inventoryRepository = inventoryRepository;
            _bookRepository = bookRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = _mapper.Map<Order>(request);

                order.Status = "Pending";
                order.UpdateAt = DateTime.UtcNow;
                order.Price = 0;
                order.IsDeleted = false;

                await _orderRepository.InsertAsync(order);

                decimal totalPrice = 0;

                foreach (var item in request.Items)
                {
                    var inventoryItem = await _inventoryRepository.FirstOrDefaultAsync(
                        i => i.BookID == item.BookId && i.BookStoreID == request.BookStoreID);

                    if (inventoryItem == null || inventoryItem.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Not enough stock for Book ID {item.BookId}");
                    }

                    var book = await _bookRepository.FirstOrDefaultAsync(b => b.BookID == item.BookId);

                    var detail = new OrderDetails
                    {
                        OrderNumber = order.OrderNumber,
                        BookID = item.BookId,
                        Quantity = item.Quantity,
                        PriceAtPurchase = book.Price
                    };

                    await _orderDetailsRepository.InsertAsync(detail);

                    inventoryItem.StockQuantity -= item.Quantity;
                    await _inventoryRepository.UpdateAsync(inventoryItem);

                    totalPrice += book.Price * item.Quantity;
                }

                order.Price = totalPrice;
                await _orderRepository.UpdateAsync(order);

                await transaction.CommitAsync();

                var createdOrder = await _orderRepository    
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails).ThenInclude(od => od.Book)
                    .FirstOrDefaultAsync(o => o.OrderNumber == order.OrderNumber);

                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository
                .Include(o => o.OrderDetails).ThenInclude(od => od.Book)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.UpdateAt)
                .ToListAsync();

            return _mapper.Map<List<OrderDto>>(orders);
        }
    }
}
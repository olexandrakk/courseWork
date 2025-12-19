using courseWork.BLL.Common.DTO;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetails> _orderDetailsRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Author> _authorRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<BookStore> _bookStoreRepository;
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<Publisher> _publisherRepository;

        public StatisticsService(
            IRepository<Book> bookRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderDetails> orderDetailsRepository,
            IRepository<User> userRepository,
            IRepository<Author> authorRepository,
            IRepository<Review> reviewRepository,
            IRepository<BookStore> bookStoreRepository,
            IRepository<Inventory> inventoryRepository,
            IRepository<Publisher> publisherRepository)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _userRepository = userRepository;
            _authorRepository = authorRepository;
            _reviewRepository = reviewRepository;
            _bookStoreRepository = bookStoreRepository;
            _inventoryRepository = inventoryRepository;
            _publisherRepository = publisherRepository;
        }

        public async Task<List<BookSalesStatisticDto>> GetTopBooksByRevenueAsync(int limit = 10)
        {
            var results = await _bookRepository
                .Include(b => b.Publisher)
                .Include(b => b.OrderDetails)
                    .ThenInclude(od => od.Order)
                .Where(b => !b.IsDeleted)
                .Select(b => new
                {
                    BookTitle = b.Name,
                    PublisherName = b.Publisher.Name,
                    TotalCopiesSold = b.OrderDetails
                        .Where(od => !od.Order.IsDeleted)
                        .Sum(od => od.Quantity),
                    TotalRevenue = b.OrderDetails
                        .Where(od => !od.Order.IsDeleted)
                        .Sum(od => od.Quantity * od.PriceAtPurchase)
                })
                .Where(x => x.TotalRevenue > 0)
                .OrderByDescending(x => x.TotalRevenue)
                .Take(limit)
                .ToListAsync();

            return results.Select((r, index) => new BookSalesStatisticDto
            {
                BookTitle = r.BookTitle,
                PublisherName = r.PublisherName,
                TotalCopiesSold = r.TotalCopiesSold,
                TotalRevenue = r.TotalRevenue,
                Rank = index + 1
            }).ToList();
        }

        public async Task<List<UserSpendingStatisticDto>> GetTopUsersBySpendingAsync(decimal minSpending = 1000)
        {
            return await _userRepository
                .Include(u => u.Orders)
                .Where(u => !u.IsDeleted)
                .Select(u => new UserSpendingStatisticDto
                {
                    Username = u.Username,
                    Email = u.Email,
                    City = u.City,
                    TotalOrders = u.Orders.Count(o => !o.IsDeleted),
                    TotalSpent = u.Orders
                        .Where(o => !o.IsDeleted)
                        .Sum(o => o.Price),
                    AverageOrderValue = u.Orders
                        .Where(o => !o.IsDeleted)
                        .Average(o => o.Price)
                })
                .Where(x => x.TotalSpent > minSpending)
                .OrderByDescending(x => x.TotalSpent)
                .ToListAsync();
        }

        public async Task<List<AuthorReviewStatisticDto>> GetAuthorsWithReviewsAsync(int minReviews = 5)
        {
            return await _authorRepository
                .Include(a => a.Books)
                    .ThenInclude(b => b.Reviews)
                .Select(a => new
                {
                    AuthorName = a.Name,
                    BooksWritten = a.Books.Count,
                    TotalReviews = a.Books
                        .SelectMany(b => b.Reviews)
                        .Count(),
                    AverageRating = a.Books
                        .SelectMany(b => b.Reviews)
                        .Select(r => (decimal?)r.Rating)
                        .Average() ?? 0
                })
                .Where(x => x.TotalReviews > minReviews)
                .OrderByDescending(x => x.AverageRating)
                .Select(x => new AuthorReviewStatisticDto
                {
                    AuthorName = x.AuthorName,
                    BooksWritten = x.BooksWritten,
                    TotalReviews = x.TotalReviews,
                    AverageRating = Math.Round(x.AverageRating, 2)
                })
                .ToListAsync();
        }

        public async Task<List<StoreInventoryStatisticDto>> GetStoreInventoryStatisticsAsync()
        {
            return await _bookStoreRepository
                .Include(bs => bs.Inventories)
                    .ThenInclude(i => i.Book)
                .Select(bs => new StoreInventoryStatisticDto
                {
                    StoreName = bs.Name,
                    Address = bs.Address,
                    UniqueTitles = bs.Inventories
                        .Select(i => i.BookID)
                        .Distinct()
                        .Count(),
                    TotalItems = bs.Inventories
                        .Sum(i => i.StockQuantity),
                    TotalInventoryValue = bs.Inventories
                        .Sum(i => i.StockQuantity * i.Book.Price)
                })
                .OrderByDescending(x => x.TotalInventoryValue)
                .ToListAsync();
        }

        public async Task<List<LowStockBookDto>> GetLowStockBooksAsync(int maxStock = 10)
        {
            var results = await _bookRepository
                .Include(b => b.Publisher)
                .Include(b => b.Inventories)
                    .ThenInclude(i => i.BookStore)
                .Where(b => !b.IsDeleted)
                .Select(b => new
                {
                    BookID = b.BookID,
                    ISBN = b.ISBN,
                    BookTitle = b.Name,
                    Publisher = b.Publisher.Name,
                    GlobalStock = b.Inventories.Sum(i => i.StockQuantity),
                    InventoryItems = b.Inventories
                        .Select(i => new { i.BookStore.Name, i.StockQuantity })
                        .ToList()
                })
                .Where(x => x.GlobalStock < maxStock)
                .OrderBy(x => x.GlobalStock)
                .ToListAsync();

            return results.Select(r => new LowStockBookDto
            {
                BookID = r.BookID,
                ISBN = r.ISBN,
                BookTitle = r.BookTitle,
                Publisher = r.Publisher,
                GlobalStock = r.GlobalStock,
                StockBreakdown = string.Join(", ", r.InventoryItems.Select(x => $"{x.Name} ({x.StockQuantity})"))
            }).ToList();
        }

        public async Task<List<FrozenAssetDto>> GetFrozenAssetsAsync()
        {
            var booksWithOrders = await _orderDetailsRepository
                .Select(od => od.BookID)
                .Distinct()
                .ToListAsync();

            return await _bookRepository
                .Include(b => b.Publisher)
                .Include(b => b.Inventories)
                .Where(b => !booksWithOrders.Contains(b.BookID))
                .Select(b => new FrozenAssetDto
                {
                    Name = b.Name,
                    Price = b.Price,
                    Publisher = b.Publisher.Name,
                    FrozenAssetsQty = b.Inventories.Sum(i => i.StockQuantity)
                })
                .OrderByDescending(x => x.FrozenAssetsQty)
                .ToListAsync();
        }
    }
}

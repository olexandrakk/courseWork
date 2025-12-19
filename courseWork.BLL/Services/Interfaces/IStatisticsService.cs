using courseWork.BLL.Common.DTO;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<List<BookSalesStatisticDto>> GetTopBooksByRevenueAsync(int limit = 10);
        Task<List<UserSpendingStatisticDto>> GetTopUsersBySpendingAsync(decimal minSpending = 1000);
        Task<List<AuthorReviewStatisticDto>> GetAuthorsWithReviewsAsync(int minReviews = 5);
        Task<List<StoreInventoryStatisticDto>> GetStoreInventoryStatisticsAsync();
        Task<List<LowStockBookDto>> GetLowStockBooksAsync(int maxStock = 10);
        Task<List<FrozenAssetDto>> GetFrozenAssetsAsync();
    }
}


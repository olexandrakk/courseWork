using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("top-books-by-revenue")]
        public async Task<IActionResult> GetTopBooksByRevenue([FromQuery] int limit = 10)
        {
            var result = await _statisticsService.GetTopBooksByRevenueAsync(limit);
            return Ok(result);
        }

        [HttpGet("top-users-by-spending")]
        public async Task<IActionResult> GetTopUsersBySpending([FromQuery] decimal minSpending = 1000)
        {
            var result = await _statisticsService.GetTopUsersBySpendingAsync(minSpending);
            return Ok(result);
        }

        [HttpGet("authors-with-reviews")]
        public async Task<IActionResult> GetAuthorsWithReviews([FromQuery] int minReviews = 5)
        {
            var result = await _statisticsService.GetAuthorsWithReviewsAsync(minReviews);
            return Ok(result);
        }

        [HttpGet("store-inventory")]
        public async Task<IActionResult> GetStoreInventoryStatistics()
        {
            var result = await _statisticsService.GetStoreInventoryStatisticsAsync();
            return Ok(result);
        }

        [HttpGet("low-stock-books")]
        public async Task<IActionResult> GetLowStockBooks([FromQuery] int maxStock = 10)
        {
            var result = await _statisticsService.GetLowStockBooksAsync(maxStock);
            return Ok(result);
        }

        [HttpGet("frozen-assets")]
        public async Task<IActionResult> GetFrozenAssets()
        {
            var result = await _statisticsService.GetFrozenAssetsAsync();
            return Ok(result);
        }
    }
}


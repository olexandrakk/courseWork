using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetByBook(int bookId)
        {
            var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewRequest request)
        {
            var review = await _reviewService.CreateReviewAsync(request);
            return Ok(review);
        }
    }
}
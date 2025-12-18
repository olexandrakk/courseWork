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

        [HttpPost("get-by-book")]
        public async Task<IActionResult> GetByBook([FromBody] GetReviewsRequest request)
        {
            var reviews = await _reviewService.GetReviewsByBookIdAsync(request);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewRequest request)
        {
            var review = await _reviewService.CreateReviewAsync(request);
            return Ok(review);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] CreateReviewRequest request)
        {
            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(id, request);
                return Ok(updatedReview);
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Common.Requests.Pagination;
using courseWork.BLL.Extensions;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IRepository<Review> reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<ReviewDto> AddReviewAsync(CreateReviewRequest request)
        {

            var review = _mapper.Map<Review>(request);
            review.Date = DateTime.UtcNow;

            await _reviewRepository.InsertAsync(review);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewRequest request)
        {
            var review = _mapper.Map<Review>(request);

            review.Date = DateTime.UtcNow;

            await _reviewRepository.InsertAsync(review);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<PagedResult<ReviewDto>> GetReviewsByBookIdAsync(GetReviewsRequest request)
        {
            var query = _reviewRepository
                .Where(r => r.BookID == request.BookId)
                .OrderByDescending(r => r.Date)
                .AsQueryable();

            var pagedReviews = await query
                .ToPagedResultAsync(request.Page, request.PageSize);

            return new PagedResult<ReviewDto>
            {
                Items = _mapper.Map<List<ReviewDto>>(pagedReviews.Items),
                Page = pagedReviews.Page,
                PageSize = pagedReviews.PageSize,
                TotalCount = pagedReviews.TotalCount
            };
        }

        public async Task<ReviewDto> UpdateReviewAsync(int id, CreateReviewRequest request)
        {
            var review = await _reviewRepository
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            if (review == null)
                throw new Exception($"Review with current Id doesn't exist: {id}");

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.BookID = request.BookId;
            review.UserID = request.UserId;

            await _reviewRepository.UpdateAsync(review);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _reviewRepository
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            if (review == null)
                throw new Exception($"Review with current Id doesn't exist: {id}");

            await _reviewRepository.DeleteAsync(review);
        }
    }
}

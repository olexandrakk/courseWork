using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
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

        public async Task<List<ReviewDto>> GetReviewsByBookIdAsync(int bookId)
        {
            var allReviews = await _reviewRepository.ToListAsync();
            var filteredReviews = allReviews.Where(r => r.BookID == bookId).ToList();

            return _mapper.Map<List<ReviewDto>>(filteredReviews);
        }
    }
}

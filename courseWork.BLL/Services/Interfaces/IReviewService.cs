using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Common.Requests.Pagination;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(CreateReviewRequest request);
        Task<PagedResult<ReviewDto>> GetReviewsByBookIdAsync(GetReviewsRequest request);
        Task<ReviewDto> UpdateReviewAsync(int id, CreateReviewRequest request);
        Task DeleteReviewAsync(int id);
    }
}

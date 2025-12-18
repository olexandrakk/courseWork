using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(CreateReviewRequest request);
        Task<List<ReviewDto>> GetReviewsByBookIdAsync(int bookId);
    }
}

using courseWork.BLL.Common.Requests.Pagination;

namespace courseWork.BLL.Common.Requests
{
    public class GetReviewsRequest : PaginationRequest
    {
        public int BookId { get; init; }
    }
}


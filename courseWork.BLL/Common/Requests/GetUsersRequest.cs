using courseWork.BLL.Common.Requests.Pagination;

namespace courseWork.BLL.Common.Requests
{
    public class GetUsersRequest : PaginationRequest
    {
        public string? Email { get; init; }
    }
}


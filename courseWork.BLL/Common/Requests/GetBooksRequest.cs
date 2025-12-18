using courseWork.BLL.Common.Requests.Pagination;

namespace courseWork.BLL.Common.Requests
{
    public class GetBooksRequest : PaginationRequest
    {
        public string? Name { get; init; }
        public int? AuthorId { get; init; }
    }
}

namespace courseWork.BLL.Common.Requests.Pagination
{
    public abstract class PaginationRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}

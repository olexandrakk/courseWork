namespace courseWork.BLL.Common.DTO
{
    public class AuthorReviewStatisticDto
    {
        public string AuthorName { get; set; } = string.Empty;
        public int BooksWritten { get; set; }
        public int TotalReviews { get; set; }
        public decimal AverageRating { get; set; }
    }
}


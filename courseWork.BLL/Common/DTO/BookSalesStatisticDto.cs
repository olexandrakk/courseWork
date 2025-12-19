namespace courseWork.BLL.Common.DTO
{
    public class BookSalesStatisticDto
    {
        public string BookTitle { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;
        public int TotalCopiesSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int Rank { get; set; }
    }
}


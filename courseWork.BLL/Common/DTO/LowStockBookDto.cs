namespace courseWork.BLL.Common.DTO
{
    public class LowStockBookDto
    {
        public int BookID { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int GlobalStock { get; set; }
        public string StockBreakdown { get; set; } = string.Empty;
    }
}


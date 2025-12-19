namespace courseWork.BLL.Common.DTO
{
    public class StoreInventoryStatisticDto
    {
        public string StoreName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int UniqueTitles { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }
}


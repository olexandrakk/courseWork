namespace courseWork.BLL.Common.DTO
{
    public class OrderDetailDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}
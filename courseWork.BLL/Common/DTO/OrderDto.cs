namespace courseWork.BLL.Common.DTO
{
    public class OrderDto
    {
        public int OrderNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }

        public List<OrderDetailDto> Items { get; set; } = new List<OrderDetailDto>();
    }
}
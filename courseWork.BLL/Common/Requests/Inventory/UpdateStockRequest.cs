namespace courseWork.BLL.Common.Requests.Inventory
{
    public class UpdateStockRequest
    {
        public int BookId { get; set; }
        public int BookStoreId { get; set; }
        public int NewQuantity { get; set; }
    }
}

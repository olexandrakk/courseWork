namespace courseWork.BLL.Common.DTO
{
    public class InventoryDto
{
    public int InventoryID { get; set; }

    public int BookID { get; set; }
    public int BookStoreID { get; set; }

    public string BookTitle { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;

    public int StockQuantity { get; set; }
}
}
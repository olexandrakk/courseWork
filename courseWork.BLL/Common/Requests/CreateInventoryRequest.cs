using courseWork.DAL.Entities;

namespace courseWork.BLL.Common.Requests
{
    public class CreateInventoryRequest
    {
        public int BookID { get; set; }
        public Book Book { get; set; }

        public int BookStoreID { get; set; }
        public BookStore BookStore { get; set; }

        public int StockQuantity { get; set; }
    }
}

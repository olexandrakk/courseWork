using System.ComponentModel.DataAnnotations;

namespace courseWork.DAL.Entities
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }

        public int BookStoreID { get; set; }
        public BookStore BookStore { get; set; }

        public int StockQuantity { get; set; }
    }
}

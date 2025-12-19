using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace courseWork.DAL.Entities
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookID { get; set; }
        public Book Book { get; set; }

        [ForeignKey(nameof(BookStoreID))]
        public int BookStoreID { get; set; }
        public BookStore BookStore { get; set; }

        public int StockQuantity { get; set; }
    }
}

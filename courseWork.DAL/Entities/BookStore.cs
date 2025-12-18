using System.ComponentModel.DataAnnotations;

namespace courseWork.DAL.Entities
{
    public class BookStore
    {
        [Key]
        public int BookStoreID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        // Навігація
        public List<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork.DAL.Entities
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        public string ISBN { get; set; }
        public string Name { get; set; }
        public int NumberOfPages { get; set; }
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int PublisherID { get; set; }
        public Publisher Publisher { get; set; }

        public List<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public List<Inventory> Inventories { get; set; } = new List<Inventory>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}

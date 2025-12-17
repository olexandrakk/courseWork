using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork.DAL.Entities
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }
        public int OrderNumber { get; set; } // FK на Order
        public Order Order { get; set; }

        public int BookID { get; set; } // FK на Book
        public Book Book { get; set; }

        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}

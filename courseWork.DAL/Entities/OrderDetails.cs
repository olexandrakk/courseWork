using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork.DAL.Entities
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderNumber { get; set; }
        public Order Order { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookID { get; set; }
        public Book Book { get; set; }

        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}

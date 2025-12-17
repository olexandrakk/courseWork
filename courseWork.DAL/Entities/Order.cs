using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork.DAL.Entities
{
    public class Order
    {
        [Key]
        public int OrderNumber { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }

        public bool is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
        public DateTime? update_at { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
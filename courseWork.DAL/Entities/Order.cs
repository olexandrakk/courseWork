using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace courseWork.DAL.Entities
{
    public class Order
    {
        [Key]
        public int OrderNumber { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        [ForeignKey(nameof(UserID))]
        public int UserID { get; set; }
        public User User { get; set; }

        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
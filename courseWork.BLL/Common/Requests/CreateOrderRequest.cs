using System.ComponentModel.DataAnnotations;

namespace courseWork.BLL.Common.Requests
{
    public class CreateOrderRequest
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int BookStoreID { get; set; } 

        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>(); 
    }

    public class OrderItemRequest
    {
        [Required]
        public int BookId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
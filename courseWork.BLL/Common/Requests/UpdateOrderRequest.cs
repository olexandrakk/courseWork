using System.ComponentModel.DataAnnotations;

namespace courseWork.BLL.Common.Requests
{
    public class UpdateOrderRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}


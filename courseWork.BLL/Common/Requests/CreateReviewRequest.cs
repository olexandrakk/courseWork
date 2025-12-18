using System.ComponentModel.DataAnnotations;

namespace courseWork.BLL.Common.Requests
{
    public class CreateReviewRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Range(1, 5)] 
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;
    }
}
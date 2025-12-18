using System.ComponentModel.DataAnnotations;

namespace courseWork.BLL.Common.Requests
{
    public class CreateBookRequest
    {
        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Name { get; set; }

        public int NumberOfPages { get; set; }
        public decimal Price { get; set; }

        [Required]
        public int PublisherID { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Book must have at least one author.")]
        public List<int> AuthorIds { get; set; } = new List<int>();
    }
}

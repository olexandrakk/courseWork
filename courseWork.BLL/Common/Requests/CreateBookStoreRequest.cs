using System.ComponentModel.DataAnnotations;

namespace courseWork.BLL.Common.Requests
{
    public class CreateBookStoreRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
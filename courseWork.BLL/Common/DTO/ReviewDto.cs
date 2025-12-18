using courseWork.DAL.Entities;

namespace courseWork.BLL.Common.DTO
{
    public class ReviewDto
    {
        public int ReviewID { get; set; } 
        public int Rating { get; set; } 
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int BookID { get; set; }
        public Book Book { get; set; }
    }
}

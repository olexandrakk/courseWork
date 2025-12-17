using System.ComponentModel.DataAnnotations;

namespace courseWork.DAL.Entities
{
    public class Author
    {
        [Key]
        public int AuthorID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public List<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
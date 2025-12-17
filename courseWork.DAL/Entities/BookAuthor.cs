using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork.DAL.Entities
{
    public class BookAuthor
    {
        [Key]
        public int BookAuthorID { get; set; }

        public int AuthorID { get; set; }
        public Author Author { get; set; }

        public int BookID { get; set; }
        public Book Book { get; set; }
    }
}

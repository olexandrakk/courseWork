using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork.DAL.Entities
{
    public class Publisher
    {
        [Key]
        public int PublisherID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
    }
}

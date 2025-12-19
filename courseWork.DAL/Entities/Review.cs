using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork.DAL.Entities
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookID { get; set; }
        public Book Book { get; set; }
    }
}
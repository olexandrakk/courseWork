using System;
using System.ComponentModel.DataAnnotations;

namespace courseWork.DAL.Entities
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
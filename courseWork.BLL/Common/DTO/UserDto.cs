namespace courseWork.BLL.Common.DTO
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

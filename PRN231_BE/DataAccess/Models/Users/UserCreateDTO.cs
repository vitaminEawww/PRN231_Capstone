using DataAccess.Enums;

namespace DataAccess.Models.Users
{
    public class UserCreateDTO
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
    }
}

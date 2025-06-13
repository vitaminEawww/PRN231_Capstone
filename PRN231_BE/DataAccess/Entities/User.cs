using DataAccess.Enums;

namespace DataAccess.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }

    public DateTime JoinDate { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual Coach? Coach { get; set; }
}

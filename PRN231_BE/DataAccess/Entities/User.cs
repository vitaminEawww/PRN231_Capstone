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
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public bool IsEmailVerified { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedUntil { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual Coach? Coach { get; set; }
}

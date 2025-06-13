using System;

namespace DataAccess.Models.Auth;

public class UserInfoDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime JoinDate { get; set; }
    public CustomerInfoDTO? Customer { get; set; }
    public CoachInfoDTO? Coach { get; set; }
}

public class CustomerInfoDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public bool IsNotificationEnabled { get; set; }
}

public class CoachInfoDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Bio { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsAvailable { get; set; }
    public float Rating { get; set; }
    public int TotalConsultations { get; set; }
}

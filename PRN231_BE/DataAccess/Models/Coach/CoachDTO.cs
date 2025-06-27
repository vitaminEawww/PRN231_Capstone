namespace DataAccess.Models.Coach;

public class CoachDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Bio { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public string Qualifications { get; set; } = null!;
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsAvailable { get; set; }
    public float Rating { get; set; }
    public int TotalConsultations { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 
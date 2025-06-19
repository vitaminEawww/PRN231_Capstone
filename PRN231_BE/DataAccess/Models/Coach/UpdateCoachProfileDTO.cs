namespace DataAccess.Models.Coach;

public class UpdateCoachProfileDTO
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Specialization { get; set; }
    public string? Qualifications { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? HourlyRate { get; set; }
    public bool? IsAvailable { get; set; }
} 
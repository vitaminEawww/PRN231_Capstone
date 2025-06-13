namespace DataAccess.Entities;

public class Coach
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Bio { get; set; } = null!;
    public string Specialization { get; set; } = null!; // Chuyên môn
    public string Qualifications { get; set; } = null!; // Bằng cấp
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsAvailable { get; set; } = true;
    public float Rating { get; set; } = 0;
    public int TotalConsultations { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    public virtual RatingSummary? RatingSummary { get; set; }
}

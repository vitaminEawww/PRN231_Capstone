using DataAccess.Enums;

namespace DataAccess.Entities;

public class Badge
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string IconUrl { get; set; } = null!;
    public BadgeType Type { get; set; }
    public string Criteria { get; set; } = null!; // Điều kiện đạt được
    public int Points { get; set; } = 0; // Điểm thưởng
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}

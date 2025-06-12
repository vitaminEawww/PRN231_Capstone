using System;

namespace DataAccess.Entities.AlternativeVersion;

public class UserBadge
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int BadgeId { get; set; }
    public DateTime EarnedAt { get; set; }
    public bool IsShared { get; set; } = false;
    public DateTime? SharedAt { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual Badge? Badge { get; set; }
}

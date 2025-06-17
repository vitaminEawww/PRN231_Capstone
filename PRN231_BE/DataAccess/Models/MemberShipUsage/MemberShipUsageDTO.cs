namespace DataAccess.Models.MemberShipUsage;

public class MemberShipUsageDTO
{
    public int CustomerId { get; set; }
    public int MembershipPackageId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}

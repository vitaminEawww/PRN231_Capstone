namespace DataAccess.Models.MemberShipUsage;

public class MemberShipUsageDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int MembershipPackageId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } 
    public string? CustomerName { get; set; }
    public string? PackageName { get; set; }
}

namespace DataAccess.Models.MembershipPackage;

public class UpdateMembershipPackageDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public bool IsActive { get; set; }
}

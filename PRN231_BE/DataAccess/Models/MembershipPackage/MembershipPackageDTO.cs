namespace DataAccess.Models.MemberShipPackage;

public class MembershipPackageDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }  
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
}

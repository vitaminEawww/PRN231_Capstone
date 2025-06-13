namespace DataAccess.Entities;

public class MembershipPackage
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; //* VD: MembershipPackageName: enum / hoặc tự đặt tên
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<MemberShipUsage> MemberShipUsages { get; set; } = new List<MemberShipUsage>();
    public virtual RatingSummary? RatingSummary { get; set; }
}

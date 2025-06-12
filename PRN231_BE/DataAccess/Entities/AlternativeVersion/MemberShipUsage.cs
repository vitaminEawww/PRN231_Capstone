using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

public class MemberShipUsage
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int MembershipPackageId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PackageStatus Status { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual MembershipPackage? MembershipPackage { get; set; }
}

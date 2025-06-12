using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

public partial class Payment
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int? MembershipPackageId { get; set; }
    public int? ConsultationId { get; set; }
    public decimal Amount { get; set; }
    public PaymentType Type { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? Description { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual MembershipPackage? MembershipPackage { get; set; }
    public virtual Consultation? Consultation { get; set; }
}

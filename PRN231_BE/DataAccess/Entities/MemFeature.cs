using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class MemFeature
{
    [Key]
    public int Id { get; set; }

    public int MembershipId { get; set; }

    public int FeatureId { get; set; }

    // Navigation properties
    [ForeignKey("MembershipId")]
    public virtual Membership Membership { get; set; }

    [ForeignKey("FeatureId")]
    public virtual Feature Feature { get; set; }
}
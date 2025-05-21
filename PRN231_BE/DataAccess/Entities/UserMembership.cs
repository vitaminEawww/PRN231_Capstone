using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class UserMembership
    {
        [Key]
        public int Id { get; set; }

        public int MembershipId { get; set; }

        public int UserId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties
        [ForeignKey("MembershipId")]
        public virtual Membership Membership { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}
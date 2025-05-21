using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class UserPlan
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PlanId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("PlanId")]
        public virtual Plan Plan { get; set; }
    }
}
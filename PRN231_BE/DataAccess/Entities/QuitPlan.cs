using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class QuitPlan
    {
        [Key]
        public int PlanID { get; set; }

        [Required]
        public int UserID { get; set; }

        public string Reason { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string CustomPlan { get; set; }

        public bool Status { get; set; } = true;

        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Achievement
    {
        [Key]
        public int AchievementID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateEarned { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        public User User { get; set; }
    }
}

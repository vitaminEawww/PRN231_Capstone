using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class SmokingLog
    {
        [Key]
        public int LogID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public int CigarettesCount { get; set; }

        public decimal Cost { get; set; }

        public string HealthStatus { get; set; }

        public bool Status { get; set; } = true;

        public User User { get; set; }
    }
}

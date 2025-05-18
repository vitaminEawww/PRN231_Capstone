using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class ProgressTracking
   {
        [Key]
        public int ProgressID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int CigarettesAvoided { get; set; }

        public decimal MoneySaved { get; set; }

        public string HealthImprovement { get; set; }

        public bool Status { get; set; } = true;

        public User User { get; set; }
    }
}

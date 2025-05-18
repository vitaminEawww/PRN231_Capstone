using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Trainer
    {
        [Key]
        public int TrainerID { get; set; }

        public string Name { get; set; }

        public string Specialization { get; set; }

        public string Contact { get; set; }

        public bool Status { get; set; } = true;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class BlogPost
    {
        [Key]
        public int PostID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        public int Rating { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        public User User { get; set; }
    }
}

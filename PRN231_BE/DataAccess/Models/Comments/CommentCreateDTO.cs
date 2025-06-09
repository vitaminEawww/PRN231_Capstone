using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Comments
{
    public class CommentCreateDTO
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }

    }
}

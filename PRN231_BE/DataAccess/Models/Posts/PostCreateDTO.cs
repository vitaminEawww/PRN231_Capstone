using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Posts
{
    public class PostCreateDTO
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}

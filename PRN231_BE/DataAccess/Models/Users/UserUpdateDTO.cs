using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Users
{
    public class UserUpdateDTO
    {
        public int UserID { get; set; } 
        public string FullName { get; set; }
        public string Username { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.MemFeatures
{
    public class MemFeatureDTO
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public int FeatureId { get; set; }
    }
}

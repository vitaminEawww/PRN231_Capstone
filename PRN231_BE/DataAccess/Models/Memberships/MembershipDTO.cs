using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Memberships
{
    public class MembershipDTO
    {
        public int PlanID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool Status { get; set; }
        public List<FeatureDTO> Features { get; set; }
    }

    public class MembershipCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Duration { get; set; }

        public List<int> FeatureIds { get; set; }
    }

    public class MembershipUpdateDTO
    {
        public int PlanID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Duration { get; set; }

        public bool Status { get; set; }

        public List<int> FeatureIds { get; set; }
    }

    public class FeatureDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
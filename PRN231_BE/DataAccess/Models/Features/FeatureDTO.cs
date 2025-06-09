using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Features
{
    public class FeatureDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class FeatureCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
    }

    public class FeatureUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
    }

    public class MemFeatureDTO
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public int FeatureId { get; set; }
        public FeatureDTO Feature { get; set; }
    }

    public class MemFeatureCreateDTO
    {
        [Required]
        public int MembershipId { get; set; }

        [Required]
        public int FeatureId { get; set; }
    }
}
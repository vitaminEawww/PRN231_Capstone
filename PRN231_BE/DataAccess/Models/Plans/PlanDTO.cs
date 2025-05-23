using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Plans
{
    public class PlanDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PhaseDTO> Phases { get; set; }
    }

    public class PlanCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public List<PhaseCreateDTO> Phases { get; set; }
    }

    public class PlanUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public List<PhaseUpdateDTO> Phases { get; set; }
    }

    public class PhaseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationDays { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EndDate { get; set; }
        public double IsCompleted { get; set; }
        public int PlanId { get; set; }
    }

    public class PhaseCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DurationDays { get; set; }
    }

    public class PhaseUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DurationDays { get; set; }

        public double IsCompleted { get; set; }
    }
}
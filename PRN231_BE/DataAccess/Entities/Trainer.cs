using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Trainer
{
    [Key]
    public int TrainerID { get; set; }

    public string Name { get; set; }

    public string Specialization { get; set; }

    public string Contact { get; set; }

    public bool Status { get; set; } = true;
}

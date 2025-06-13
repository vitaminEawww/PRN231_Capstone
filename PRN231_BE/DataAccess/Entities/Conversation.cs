using DataAccess.Enums;

namespace DataAccess.Entities;

public partial class Conversation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CoachId { get; set; }
    public int? ConsultationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public ConversationStatus Status { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual Coach? Coach { get; set; }
    public virtual Consultation? Consultation { get; set; }
    public virtual ICollection<Message>? Messages { get; set; }
}

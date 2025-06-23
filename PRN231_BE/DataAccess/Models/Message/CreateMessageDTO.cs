using DataAccess.Enums;

namespace DataAccess.Models.Message;

public class CreateMessageDTO
{
    public int? ConversationId { get; set; }
    public int CustomerId { get; set; }
    public int CoachId { get; set; }
    public int? ConsultationId { get; set; }
    public int SenderId { get; set; }
    public SenderType SenderType { get; set; }
    public string Content { get; set; } = null!;
    public MessageType Type { get; set; } = MessageType.Text;
} 
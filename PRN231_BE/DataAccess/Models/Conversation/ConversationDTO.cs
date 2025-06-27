using System;

namespace DataAccess.Models.Conversation;

public class ConversationDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CoachId { get; set; }
    public int? ConsultationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public string Status { get; set; } = null!;
} 
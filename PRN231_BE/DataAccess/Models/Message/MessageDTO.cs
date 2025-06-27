using System;
using DataAccess.Enums;

namespace DataAccess.Models.Message;

public class MessageDTO
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public SenderType SenderType { get; set; }
    public string Content { get; set; } = null!;
    public MessageType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
} 
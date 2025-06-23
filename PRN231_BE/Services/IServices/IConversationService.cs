using DataAccess.Models.Conversation;
using DataAccess.Models.Message;

public interface IConversationService
{
    Task<List<ConversationDTO>> GetUserConversationsAsync(int userId, string role);
    Task<List<MessageDTO>> GetMessagesByConversationIdAsync(int conversationId);
    Task<MessageDTO> CreateMessageAsync(CreateMessageDTO dto);
    Task<ConversationDTO> GetOrCreateConversationByConsultationAsync(int consultationId);
} 
using DataAccess.Entities;
using DataAccess.Models.Conversation;
using DataAccess.Models.Message;
using Repositories.IRepositories;
using Mapster;

public class ConversationService : IConversationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ConversationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ConversationDTO>> GetUserConversationsAsync(int userId, string role)
    {
        IEnumerable<Conversation> conversations;
        if (role.ToLower() == "customer")
        {
            // Lấy CustomerId từ UserId
            var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
                return new List<ConversationDTO>();
                
            conversations = await _unitOfWork.Conversations.GetAllAsync(c => c.CustomerId == customer.Id);
        }
        else if (role.ToLower() == "coach")
        {
            // Lấy CoachId từ UserId
            var coach = await _unitOfWork.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
            if (coach == null)
                return new List<ConversationDTO>();
                
            conversations = await _unitOfWork.Conversations.GetAllAsync(c => c.CoachId == coach.Id);
        }
        else
        {
            conversations = new List<Conversation>();
        }
        return conversations.OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt).Adapt<List<ConversationDTO>>();
    }

    public async Task<List<MessageDTO>> GetMessagesByConversationIdAsync(int conversationId)
    {
        var messages = await _unitOfWork.Messages.GetAllAsync(m => m.ConversationId == conversationId);
        return messages.OrderBy(m => m.SentAt).Adapt<List<MessageDTO>>();
    }

    public async Task<MessageDTO> CreateMessageAsync(CreateMessageDTO dto)
    {
        int conversationId;
        if (dto.ConversationId.HasValue)
        {
            conversationId = dto.ConversationId.Value;
        }
        else
        {
            // Tìm conversation giữa 2 user (và consultation nếu có)
            var conversation = await _unitOfWork.Conversations.FirstOrDefaultAsync(c =>
                c.CustomerId == dto.CustomerId &&
                c.CoachId == dto.CoachId &&
                c.ConsultationId == dto.ConsultationId);
            if (conversation == null)
            {
                // Tạo mới conversation
                conversation = new Conversation
                {
                    CustomerId = dto.CustomerId,
                    CoachId = dto.CoachId,
                    ConsultationId = dto.ConsultationId,
                    CreatedAt = DateTime.UtcNow,
                    Status = DataAccess.Enums.ConversationStatus.Active
                };
                await _unitOfWork.Conversations.AddAsync(conversation);
                await _unitOfWork.SaveAsync();
            }
            conversationId = conversation.Id;
        }

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = dto.SenderId,
            SenderType = dto.SenderType,
            Content = dto.Content,
            Type = dto.Type,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
        await _unitOfWork.Messages.AddAsync(message);

        // Cập nhật LastMessageAt cho Conversation
        var conv = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
        if (conv != null)
        {
            conv.LastMessageAt = message.SentAt;
            _unitOfWork.Conversations.Update(conv);
        }

        await _unitOfWork.SaveAsync();
        return message.Adapt<MessageDTO>();
    }

    public async Task<ConversationDTO> GetOrCreateConversationByConsultationAsync(int consultationId)
    {
        // Tìm consultation
        var consultation = await _unitOfWork.Consultations.GetByIdAsync(consultationId);
        if (consultation == null)
            throw new Exception("Consultation không tồn tại");

        // Tìm conversation theo consultationId
        var conversation = await _unitOfWork.Conversations.FirstOrDefaultAsync(c => c.ConsultationId == consultationId);
        if (conversation == null)
        {
            conversation = new Conversation
            {
                CustomerId = consultation.CustomerId,
                CoachId = consultation.CoachId,
                ConsultationId = consultationId,
                CreatedAt = DateTime.UtcNow,
                Status = DataAccess.Enums.ConversationStatus.Active
            };
            await _unitOfWork.Conversations.AddAsync(conversation);
            await _unitOfWork.SaveAsync();
        }
        return conversation.Adapt<ConversationDTO>();
    }
} 
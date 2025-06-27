using DataAccess.Models.Coach;
using DataAccess.Models.Consultation;
using DataAccess.Enums;

public interface ICoachService
{
    Task<List<CoachDTO>> GetAllCoachesAsync();
    Task<CoachDTO?> GetCoachByIdAsync(int coachId);
    Task<CoachDTO?> GetCoachProfileAsync(int userId);
    Task<CoachDTO> UpdateCoachProfileAsync(int userId, UpdateCoachProfileDTO dto);
    Task<List<ConsultationDTO>> GetCoachConsultationsAsync(int userId, ConsultationStatus? status = null);
    Task<ConsultationDTO?> GetCoachConsultationByIdAsync(int userId, int consultationId);
    Task<bool> UpdateConsultationStatusAsync(int userId, int consultationId, UpdateConsultationStatusDTO dto);
} 
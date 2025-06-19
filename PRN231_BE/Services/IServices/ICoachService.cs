using DataAccess.Models.Coach;
using DataAccess.Models.Consultation;
using DataAccess.Enums;

public interface ICoachService
{
    Task<List<CoachDTO>> GetAllCoachesAsync();
    Task<CoachDTO?> GetCoachByIdAsync(int coachId);
    Task<CoachDTO?> GetCoachProfileAsync(int coachId);
    Task<CoachDTO> UpdateCoachProfileAsync(int coachId, UpdateCoachProfileDTO dto);
    Task<List<ConsultationDTO>> GetCoachConsultationsAsync(int coachId, ConsultationStatus? status = null);
    Task<ConsultationDTO?> GetCoachConsultationByIdAsync(int coachId, int consultationId);
    Task<bool> UpdateConsultationStatusAsync(int coachId, int consultationId, UpdateConsultationStatusDTO dto);
} 
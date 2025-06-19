using DataAccess.Models.Consultation;
using DataAccess.Enums;

public interface IConsultationService
{
    Task<ConsultationDTO> CreateConsultationAsync(int customerId, CreateConsultationDTO dto);
    Task<List<ConsultationDTO>> GetConsultationsAsync(int userId, string role, ConsultationStatus? status = null);
    Task<ConsultationDTO?> GetConsultationByIdAsync(int userId, string role, int id);
    Task<bool> UpdateConsultationStatusAsync(int userId, string role, int id, UpdateConsultationStatusDTO dto);
    Task<bool> DeleteConsultationAsync(int userId, string role, int id);
} 
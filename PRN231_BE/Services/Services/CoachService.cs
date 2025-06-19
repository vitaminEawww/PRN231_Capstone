using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.Coach;
using DataAccess.Models.Consultation;
using Repositories.IRepositories;
using Mapster;

public class CoachService : ICoachService
{
    private readonly IUnitOfWork _unitOfWork;

    public CoachService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CoachDTO>> GetAllCoachesAsync()
    {
        var coaches = await _unitOfWork.Coaches.GetAllAsync(c => c.User);
        return coaches.Adapt<List<CoachDTO>>();
    }

    public async Task<CoachDTO?> GetCoachByIdAsync(int coachId)
    {
        var coach = await _unitOfWork.Coaches.GetByIdAsync(coachId, c => c.User);
        return coach?.Adapt<CoachDTO>();
    }

    public async Task<CoachDTO?> GetCoachProfileAsync(int coachId)
    {
        return await GetCoachByIdAsync(coachId);
    }

    public async Task<CoachDTO> UpdateCoachProfileAsync(int coachId, UpdateCoachProfileDTO dto)
    {
        var coach = await _unitOfWork.Coaches.GetByIdAsync(coachId);
        if (coach == null)
            throw new Exception("Coach không tồn tại");

        // Cập nhật thông tin
        if (dto.FullName != null) coach.FullName = dto.FullName;
        if (dto.Phone != null) coach.Phone = dto.Phone;
        if (dto.AvatarUrl != null) coach.AvatarUrl = dto.AvatarUrl;
        if (dto.Bio != null) coach.Bio = dto.Bio;
        if (dto.Specialization != null) coach.Specialization = dto.Specialization;
        if (dto.Qualifications != null) coach.Qualifications = dto.Qualifications;
        if (dto.ExperienceYears.HasValue) coach.ExperienceYears = dto.ExperienceYears.Value;
        if (dto.HourlyRate.HasValue) coach.HourlyRate = dto.HourlyRate.Value;
        if (dto.IsAvailable.HasValue) coach.IsAvailable = dto.IsAvailable.Value;

        coach.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Coaches.Update(coach);
        await _unitOfWork.SaveAsync();

        return await GetCoachByIdAsync(coachId) ?? throw new Exception("Không thể lấy thông tin coach sau khi cập nhật");
    }

    public async Task<List<ConsultationDTO>> GetCoachConsultationsAsync(int coachId, ConsultationStatus? status = null)
    {
        var consultations = await _unitOfWork.Consultations.GetAllAsync(c => c.CoachId == coachId);

        if (status.HasValue)
        {
            consultations = consultations.Where(c => c.Status == status.Value).ToList();
        }

        var orderedConsultations = consultations.OrderByDescending(c => c.CreatedAt).ToList();
        return orderedConsultations.Adapt<List<ConsultationDTO>>();
    }

    public async Task<ConsultationDTO?> GetCoachConsultationByIdAsync(int coachId, int consultationId)
    {
        var consultation = await _unitOfWork.Consultations.FirstOrDefaultAsync(c => c.CoachId == coachId && c.Id == consultationId);
        return consultation?.Adapt<ConsultationDTO>();
    }

    public async Task<bool> UpdateConsultationStatusAsync(int coachId, int consultationId, UpdateConsultationStatusDTO dto)
    {
        var consultation = await _unitOfWork.Consultations.FirstOrDefaultAsync(c => c.CoachId == coachId && c.Id == consultationId);

        if (consultation == null)
            throw new Exception("Buổi tư vấn không tồn tại");

        if (consultation.Status == ConsultationStatus.Completed || consultation.Status == ConsultationStatus.Cancelled)
            throw new Exception("Không thể cập nhật trạng thái buổi tư vấn đã hoàn thành hoặc đã hủy");

        consultation.Status = dto.Status;
        consultation.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Consultations.Update(consultation);
        await _unitOfWork.SaveAsync();
        return true;
    }
} 
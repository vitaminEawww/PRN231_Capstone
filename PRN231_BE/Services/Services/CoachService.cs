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

    public async Task<CoachDTO?> GetCoachProfileAsync(int userId)
    {
        var coach = await _unitOfWork.Coaches.FirstOrDefaultAsync(c => c.UserId == userId, c => c.User);
        return coach?.Adapt<CoachDTO>();
    }

    public async Task<CoachDTO> UpdateCoachProfileAsync(int userId, UpdateCoachProfileDTO dto)
    {
        var coach = await _unitOfWork.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
        if (coach == null)
            throw new Exception("Coach không tồn tại");

        if (dto.FullName != null) coach.FullName = dto.FullName;
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

        return await GetCoachByIdAsync(coach.Id) ?? throw new Exception("Không thể lấy thông tin coach sau khi cập nhật");
    }

    public async Task<List<ConsultationDTO>> GetCoachConsultationsAsync(int userId, ConsultationStatus? status = null)
    {
        var coach = await _unitOfWork.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
        if (coach == null)
            return new List<ConsultationDTO>();

        var consultations = await _unitOfWork.Consultations.GetAllAsync(c => c.CoachId == coach.Id);

        if (status.HasValue)
        {
            consultations = consultations.Where(c => c.Status == status.Value).ToList();
        }

        var orderedConsultations = consultations.OrderByDescending(c => c.CreatedAt).ToList();
        return orderedConsultations.Adapt<List<ConsultationDTO>>();
    }

    public async Task<ConsultationDTO?> GetCoachConsultationByIdAsync(int userId, int consultationId)
    {
        var coach = await _unitOfWork.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
        if (coach == null)
            return null;

        var consultation = await _unitOfWork.Consultations.FirstOrDefaultAsync(c => c.CoachId == coach.Id && c.Id == consultationId);
        return consultation?.Adapt<ConsultationDTO>();
    }

    public async Task<bool> UpdateConsultationStatusAsync(int userId, int consultationId, UpdateConsultationStatusDTO dto)
    {
        var coach = await _unitOfWork.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
        if (coach == null)
            throw new Exception("Coach không tồn tại");

        var consultation = await _unitOfWork.Consultations.FirstOrDefaultAsync(c => c.CoachId == coach.Id && c.Id == consultationId);

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
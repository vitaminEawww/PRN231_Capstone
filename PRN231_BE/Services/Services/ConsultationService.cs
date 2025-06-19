using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.Consultation;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using Mapster;

public class ConsultationService : IConsultationService
{
    private readonly ApplicationDbContext _context;

    public ConsultationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultationDTO> CreateConsultationAsync(int customerId, CreateConsultationDTO dto)
    {
        // Kiểm tra Coach tồn tại
        var coach = await _context.Coaches.FindAsync(dto.CoachId);
        if (coach == null)
            throw new Exception("Coach không tồn tại");

        if (dto.ScheduledAt <= DateTime.UtcNow)
            throw new Exception("Thời gian đặt lịch không hợp lệ");

        bool isBusy = await _context.Consultations.AnyAsync(c =>
            c.CoachId == dto.CoachId &&
            c.Status != ConsultationStatus.Cancelled &&
            c.Status != ConsultationStatus.Completed &&
            c.ScheduledAt < dto.ScheduledAt.AddMinutes(dto.DurationMinutes) &&
            c.ScheduledAt.AddMinutes(c.DurationMinutes) > dto.ScheduledAt
        );
        if (isBusy)
            throw new Exception("Huấn luyện viên đã có lịch vào thời gian này");

        var consultation = dto.Adapt<Consultation>();
        consultation.CustomerId = customerId;
        consultation.Status = ConsultationStatus.Scheduled;
        consultation.Type = ConsultationType.Chat; 
        consultation.Amount = 0;
        consultation.CreatedAt = DateTime.UtcNow;

        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync();

        // Sử dụng Mapster để mapping sang DTO trả về
        return consultation.Adapt<ConsultationDTO>();
    }

    public async Task<List<ConsultationDTO>> GetConsultationsAsync(int userId, string role, ConsultationStatus? status = null)
    {
        var query = _context.Consultations.AsQueryable();

        // Lọc theo vai trò
        if (role.ToLower() == "customer")
        {
            query = query.Where(c => c.CustomerId == userId);
        }
        else if (role.ToLower() == "coach")
        {
            query = query.Where(c => c.CoachId == userId);
        }

        // Lọc theo trạng thái nếu có
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        // Sắp xếp theo thời gian tạo mới nhất
        query = query.OrderByDescending(c => c.CreatedAt);

        var consultations = await query.ToListAsync();
        return consultations.Adapt<List<ConsultationDTO>>();
    }

    public async Task<ConsultationDTO?> GetConsultationByIdAsync(int userId, string role, int id)
    {
        var query = _context.Consultations.AsQueryable();

        // Kiểm tra quyền truy cập
        if (role.ToLower() == "customer")
        {
            query = query.Where(c => c.CustomerId == userId && c.Id == id);
        }
        else if (role.ToLower() == "coach")
        {
            query = query.Where(c => c.CoachId == userId && c.Id == id);
        }

        var consultation = await query.FirstOrDefaultAsync();
        return consultation?.Adapt<ConsultationDTO>();
    }

    public async Task<bool> UpdateConsultationStatusAsync(int userId, string role, int id, UpdateConsultationStatusDTO dto)
    {
        var consultation = await _context.Consultations.FindAsync(id);
        if (consultation == null)
            throw new Exception("Buổi tư vấn không tồn tại");

        // Kiểm tra quyền cập nhật
        if (role.ToLower() == "customer" && consultation.CustomerId != userId)
            throw new Exception("Không có quyền cập nhật buổi tư vấn này");
        if (role.ToLower() == "coach" && consultation.CoachId != userId)
            throw new Exception("Không có quyền cập nhật buổi tư vấn này");

        // Kiểm tra trạng thái hiện tại có thể cập nhật không
        if (consultation.Status == ConsultationStatus.Completed || consultation.Status == ConsultationStatus.Cancelled)
            throw new Exception("Không thể cập nhật trạng thái buổi tư vấn đã hoàn thành hoặc đã hủy");

        consultation.Status = dto.Status;
        consultation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteConsultationAsync(int userId, string role, int id)
    {
        var consultation = await _context.Consultations.FindAsync(id);
        if (consultation == null)
            throw new Exception("Buổi tư vấn không tồn tại");

        // Kiểm tra quyền xóa
        if (role.ToLower() == "customer" && consultation.CustomerId != userId)
            throw new Exception("Không có quyền xóa buổi tư vấn này");
        if (role.ToLower() == "coach" && consultation.CoachId != userId)
            throw new Exception("Không có quyền xóa buổi tư vấn này");

        // Chỉ cho phép xóa khi chưa diễn ra
        if (consultation.Status != ConsultationStatus.Scheduled)
            throw new Exception("Chỉ có thể xóa buổi tư vấn chưa diễn ra");

        _context.Consultations.Remove(consultation);
        await _context.SaveChangesAsync();
        return true;
    }
} 
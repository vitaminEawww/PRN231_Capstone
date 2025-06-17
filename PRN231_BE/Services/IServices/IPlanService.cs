using System;
using DataAccess.Common;
using DataAccess.Models.Plans;

namespace Services.IServices;

public interface IPlanService
{
    // Quản lý kế hoạch cai thuốc
    Task<ApiResponse> CreateQuitPlanAsync(int customerId, QuitPlanCreateDTO dto);
    Task<ApiResponse> GetQuitPlanByIdAsync(int planId);
    Task<ApiResponse> GetCustomerQuitPlansAsync(int customerId, int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse> UpdateQuitPlanAsync(int customerId, QuitPlanUpdateDTO dto);
    Task<ApiResponse> DeleteQuitPlanAsync(int customerId, int planId);

    // Quản lý trạng thái kế hoạch
    Task<ApiResponse> StartQuitPlanAsync(int customerId, int planId);
    Task<ApiResponse> PauseQuitPlanAsync(int customerId, int planId);
    Task<ApiResponse> ResumeQuitPlanAsync(int customerId, int planId);
    Task<ApiResponse> CompleteQuitPlanAsync(int customerId, int planId);
    Task<ApiResponse> FailQuitPlanAsync(int customerId, int planId);

    // Quản lý giai đoạn
    Task<ApiResponse> GetPlanPhasesAsync(int planId);
    Task<ApiResponse> UpdatePhaseStatusAsync(int customerId, int phaseId, bool isCompleted);
    Task<ApiResponse> AddCustomPhaseAsync(int customerId, int planId, QuitPlanPhaseCreateDTO dto);
    Task<ApiResponse> UpdatePhaseAsync(int customerId, int phaseId, QuitPlanPhaseCreateDTO dto);
    Task<ApiResponse> DeletePhaseAsync(int customerId, int phaseId);

    // Hỗ trợ tự động
    Task<ApiResponse> GenerateRecommendedPlanAsync(int customerId);
    Task<ApiResponse> GetPlanStatisticsAsync(int customerId, int planId);
}

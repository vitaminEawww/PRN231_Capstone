using DataAccess.Common;
using DataAccess.Models.DailyProgress;

namespace Services.IServices;

public interface IDailyProgressService
{
    // CRUD operations
    Task<ApiResponse> CreateDailyProgressAsync(int customerId, DailyProgressCreateDTO dto);
    Task<ApiResponse> GetDailyProgressByIdAsync(int progressId);
    Task<ApiResponse> GetDailyProgressByDateAsync(int customerId, DateTime date);
    Task<ApiResponse> UpdateDailyProgressAsync(int customerId, DailyProgressUpdateDTO dto);
    Task<ApiResponse> DeleteDailyProgressAsync(int customerId, int progressId);

    // Listing and filtering
    Task<ApiResponse> GetCustomerDailyProgressAsync(int customerId, ProgressGetListDTO dto);
    Task<ApiResponse> GetSmokeFreeeDaysAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse> GetSmokingDaysAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null);

    // Statistics and analytics  
    Task<ApiResponse> GetDailyProgressStatisticsAsync(int customerId);
    Task<ApiResponse> GetWeeklyProgressSummaryAsync(int customerId, DateTime? weekStartDate = null);
    Task<ApiResponse> GetMonthlyProgressSummaryAsync(int customerId, int? year = null, int? month = null);
    Task<ApiResponse> GetCurrentStreakAsync(int customerId);

    // Health trends
    Task<ApiResponse> GetHealthTrendsAsync(int customerId, int days = 30);
    Task<ApiResponse> GetCravingAnalysisAsync(int customerId, int days = 30);
    Task<ApiResponse> GetTriggersAnalysisAsync(int customerId, int days = 30);

    // Background tasks
    Task<ApiResponse> RecalculateCustomerStatisticsAsync(int customerId);
    Task<ApiResponse> UpdateAllCustomerStatisticsAsync();
}
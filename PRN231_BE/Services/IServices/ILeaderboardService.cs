using System;
using DataAccess.Common;

namespace Services.IServices;

public interface ILeaderboardService
{
    Task<ApiResponse> UpdateAllLeaderboardsAsync();
    Task<ApiResponse> UpdateDailyLeaderboardAsync();
    Task<ApiResponse> UpdateWeeklyLeaderboardAsync();
    Task<ApiResponse> UpdateMonthlyLeaderboardAsync();
    Task<ApiResponse> GetLeaderboardAsync(string period, string type, int limit = 100);
}

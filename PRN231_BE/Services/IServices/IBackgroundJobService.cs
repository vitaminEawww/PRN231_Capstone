using System;
using DataAccess.Common;

namespace Services.IServices;

public interface IBackgroundJobService
{
    Task<ApiResponse> TriggerStatisticsUpdateJobAsync();
    Task<ApiResponse> TriggerLeaderboardUpdateJobAsync();
    Task<ApiResponse> TriggerNotificationJobAsync();
    Task<ApiResponse> GetJobStatusAsync();
    Task<ApiResponse> ScheduleCustomJobAsync(string jobType, DateTime scheduledTime);
}

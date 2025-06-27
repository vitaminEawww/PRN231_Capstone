using System.Diagnostics;
using System.Net;
using DataAccess.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.BackgroundServices;
using Services.IServices;

namespace Services.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    // Dictionary để track job status
    private static readonly Dictionary<string, JobStatus> _jobStatuses = new();
    private static readonly object _lockObject = new();

    public BackgroundJobService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BackgroundJobService> logger,
        IHostApplicationLifetime applicationLifetime)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    public async Task<ApiResponse> TriggerStatisticsUpdateJobAsync()
    {
        var response = new ApiResponse();
        try
        {
            var jobId = $"statistics-update-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

            lock (_lockObject)
            {
                _jobStatuses[jobId] = new JobStatus
                {
                    JobId = jobId,
                    JobType = "StatisticsUpdate",
                    Status = "Running",
                    StartTime = DateTime.UtcNow,
                    Message = "Đang cập nhật thống kê cho tất cả khách hàng..."
                };
            }

            // Chạy job trong background
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var dailyProgressService = scope.ServiceProvider.GetRequiredService<IDailyProgressService>();

                    var result = await dailyProgressService.UpdateAllCustomerStatisticsAsync();

                    lock (_lockObject)
                    {
                        _jobStatuses[jobId] = _jobStatuses[jobId] with
                        {
                            Status = result.IsSuccess ? "Completed" : "Failed",
                            EndTime = DateTime.UtcNow,
                            Message = result.IsSuccess ? "Cập nhật thống kê thành công" : string.Join(", ", result.ErrorMessages),
                            Result = result.Result
                        };
                    }

                    _logger.LogInformation("Manual statistics update job {JobId} completed with status: {Status}",
                        jobId, result.IsSuccess ? "Success" : "Failed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Manual statistics update job {JobId} failed", jobId);

                    lock (_lockObject)
                    {
                        _jobStatuses[jobId] = _jobStatuses[jobId] with
                        {
                            Status = "Failed",
                            EndTime = DateTime.UtcNow,
                            Message = $"Lỗi: {ex.Message}"
                        };
                    }
                }
            });

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Accepted;
            response.ErrorMessages.Add("Job cập nhật thống kê đã được khởi tạo");
            response.Result = new { JobId = jobId, Message = "Job đang chạy trong background" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering statistics update job");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi khởi tạo job");
        }

        return response;
    }

    public async Task<ApiResponse> TriggerLeaderboardUpdateJobAsync()
    {
        var response = new ApiResponse();
        try
        {
            var jobId = $"leaderboard-update-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

            lock (_lockObject)
            {
                _jobStatuses[jobId] = new JobStatus
                {
                    JobId = jobId,
                    JobType = "LeaderboardUpdate",
                    Status = "Running",
                    StartTime = DateTime.UtcNow,
                    Message = "Đang cập nhật bảng xếp hạng..."
                };
            }

            // Chạy job trong background
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var leaderboardService = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

                    // Giả sử có service này, nếu chưa có thì implement
                    await leaderboardService.UpdateAllLeaderboardsAsync();

                    lock (_lockObject)
                    {
                        _jobStatuses[jobId] = _jobStatuses[jobId] with
                        {
                            Status = "Completed",
                            EndTime = DateTime.UtcNow,
                            Message = "Cập nhật bảng xếp hạng thành công"
                        };
                    }

                    _logger.LogInformation("Manual leaderboard update job {JobId} completed successfully", jobId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Manual leaderboard update job {JobId} failed", jobId);

                    lock (_lockObject)
                    {
                        _jobStatuses[jobId] = _jobStatuses[jobId] with
                        {
                            Status = "Failed",
                            EndTime = DateTime.UtcNow,
                            Message = $"Lỗi: {ex.Message}"
                        };
                    }
                }
            });

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Accepted;
            response.ErrorMessages.Add("Job cập nhật bảng xếp hạng đã được khởi tạo");
            response.Result = new { JobId = jobId, Message = "Job đang chạy trong background" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering leaderboard update job");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi khởi tạo job");
        }

        return response;
    }

    public async Task<ApiResponse> TriggerNotificationJobAsync()
    {
        var response = new ApiResponse();
        try
        {
            var jobId = $"notification-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

            lock (_lockObject)
            {
                _jobStatuses[jobId] = new JobStatus
                {
                    JobId = jobId,
                    JobType = "NotificationSend",
                    Status = "Running",
                    StartTime = DateTime.UtcNow,
                    Message = "Đang gửi notification..."
                };
            }

            // Chạy job trong background
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    // Gửi daily reminders và kiểm tra milestones
                    var result = await notificationService.SendDailyNotificationsAsync();

                    lock (_lockObject)
                    {
                        _jobStatuses[jobId] = _jobStatuses[jobId] with
                        {
                            Status = result.IsSuccess ? "Completed" : "Failed",
                            EndTime = DateTime.UtcNow,
                            Message = result.IsSuccess ? "Gửi notification thành công" : string.Join(", ", result.ErrorMessages),
                            Result = result.Result
                        };
                    }

                    _logger.LogInformation("Manual notification job {JobId} completed with status: {Status}",
                        jobId, result.IsSuccess ? "Success" : "Failed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Manual notification job {JobId} failed", jobId);

                    lock (_lockObject)
                    {
                        _jobStatuses[jobId] = _jobStatuses[jobId] with
                        {
                            Status = "Failed",
                            EndTime = DateTime.UtcNow,
                            Message = $"Lỗi: {ex.Message}"
                        };
                    }
                }
            });

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Accepted;
            response.ErrorMessages.Add("Job gửi notification đã được khởi tạo");
            response.Result = new { JobId = jobId, Message = "Job đang chạy trong background" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering notification job");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi khởi tạo job");
        }

        return response;
    }

    public async Task<ApiResponse> GetJobStatusAsync()
    {
        var response = new ApiResponse();
        try
        {
            List<JobStatus> allJobs;
            lock (_lockObject)
            {
                allJobs = _jobStatuses.Values
                    .OrderByDescending(j => j.StartTime)
                    .Take(50) // Lấy 50 job gần nhất
                    .ToList();
            }

            // Thống kê tổng quan
            var summary = new
            {
                TotalJobs = allJobs.Count,
                RunningJobs = allJobs.Count(j => j.Status == "Running"),
                CompletedJobs = allJobs.Count(j => j.Status == "Completed"),
                FailedJobs = allJobs.Count(j => j.Status == "Failed"),
                PendingJobs = allJobs.Count(j => j.Status == "Pending")
            };

            // Group theo type
            var jobsByType = allJobs
                .GroupBy(j => j.JobType)
                .ToDictionary(g => g.Key, g => new
                {
                    Total = g.Count(),
                    Running = g.Count(j => j.Status == "Running"),
                    Completed = g.Count(j => j.Status == "Completed"),
                    Failed = g.Count(j => j.Status == "Failed"),
                    LastRun = g.OrderByDescending(j => j.StartTime).FirstOrDefault()?.StartTime
                });

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.ErrorMessages.Add("Lấy trạng thái job thành công");
            response.Result = new
            {
                Summary = summary,
                JobsByType = jobsByType,
                RecentJobs = allJobs.Take(10), // 10 job gần nhất
                SystemInfo = new
                {
                    ServerTime = DateTime.UtcNow,
                    ApplicationUptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime(),
                    MemoryUsage = GC.GetTotalMemory(false) / 1024 / 1024 // MB
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job status");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi lấy trạng thái job");
        }

        return response;
    }

    public async Task<ApiResponse> ScheduleCustomJobAsync(string jobType, DateTime scheduledTime)
    {
        var response = new ApiResponse();
        try
        {
            var jobId = $"{jobType.ToLower()}-scheduled-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

            lock (_lockObject)
            {
                _jobStatuses[jobId] = new JobStatus
                {
                    JobId = jobId,
                    JobType = jobType,
                    Status = "Scheduled",
                    StartTime = DateTime.UtcNow,
                    ScheduledTime = scheduledTime,
                    Message = $"Job được lên lịch chạy vào {scheduledTime:dd/MM/yyyy HH:mm:ss}"
                };
            }

            // Schedule job để chạy vào thời điểm định trước
            _ = Task.Run(async () =>
            {
                try
                {
                    var delay = scheduledTime - DateTime.UtcNow;
                    if (delay.TotalMilliseconds > 0)
                    {
                        await Task.Delay(delay);
                    }

                    lock (_lockObject)
                    {
                        if (_jobStatuses.ContainsKey(jobId))
                        {
                            _jobStatuses[jobId] = _jobStatuses[jobId] with
                            {
                                Status = "Running",
                                Message = "Job đang chạy..."
                            };
                        }
                    }

                    // Execute the job based on type
                    var result = jobType.ToLower() switch
                    {
                        "statisticsupdate" => await TriggerStatisticsUpdateJobAsync(),
                        "leaderboardupdate" => await TriggerLeaderboardUpdateJobAsync(),
                        "notification" => await TriggerNotificationJobAsync(),
                        _ => throw new ArgumentException($"Unsupported job type: {jobType}")
                    };

                    lock (_lockObject)
                    {
                        if (_jobStatuses.ContainsKey(jobId))
                        {
                            _jobStatuses[jobId] = _jobStatuses[jobId] with
                            {
                                Status = result.IsSuccess ? "Completed" : "Failed",
                                EndTime = DateTime.UtcNow,
                                Message = result.IsSuccess ? "Scheduled job completed successfully" : "Scheduled job failed"
                            };
                        }
                    }

                    _logger.LogInformation("Scheduled job {JobId} of type {JobType} completed", jobId, jobType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduled job {JobId} failed", jobId);

                    lock (_lockObject)
                    {
                        if (_jobStatuses.ContainsKey(jobId))
                        {
                            _jobStatuses[jobId] = _jobStatuses[jobId] with
                            {
                                Status = "Failed",
                                EndTime = DateTime.UtcNow,
                                Message = $"Scheduled job failed: {ex.Message}"
                            };
                        }
                    }
                }
            });

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Accepted;
            response.ErrorMessages.Add("Job đã được lên lịch thành công");
            response.Result = new
            {
                JobId = jobId,
                JobType = jobType,
                ScheduledTime = scheduledTime,
                Message = "Job sẽ chạy vào thời điểm đã định"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling custom job");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi lên lịch job");
        }

        return response;
    }

    // Helper method để cleanup old job statuses
    public void CleanupOldJobStatuses(int keepLastDays = 7)
    {
        lock (_lockObject)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-keepLastDays);
            var keysToRemove = _jobStatuses
                .Where(kvp => kvp.Value.StartTime < cutoffDate)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _jobStatuses.Remove(key);
            }
            _logger.LogInformation($"Cleaned up {keysToRemove.Count} old job statuses");
        }
    }
}

// Record để định nghĩa JobStatus
public record JobStatus
{
    public string JobId { get; init; } = string.Empty;
    public string JobType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty; // Pending, Running, Completed, Failed, Scheduled
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public DateTime? ScheduledTime { get; init; }
    public string Message { get; init; } = string.Empty;
    public object? Result { get; init; }
    public TimeSpan? Duration => EndTime?.Subtract(StartTime);
}
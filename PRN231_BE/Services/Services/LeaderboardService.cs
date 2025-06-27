using System.Net;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LeaderboardService> _logger;

    public LeaderboardService(
        IUnitOfWork unitOfWork,
        ILogger<LeaderboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse> UpdateAllLeaderboardsAsync()
    {
        var response = new ApiResponse();
        try
        {
            _logger.LogInformation("Starting comprehensive leaderboard update");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await UpdateDailyLeaderboardAsync();
                await UpdateWeeklyLeaderboardAsync();
                await UpdateMonthlyLeaderboardAsync();

                await _unitOfWork.CommitTransactionAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.ErrorMessages.Add("Cập nhật tất cả bảng xếp hạng thành công");

                _logger.LogInformation("Successfully updated all leaderboards");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating all leaderboards");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi cập nhật bảng xếp hạng: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> UpdateDailyLeaderboardAsync()
    {
        var response = new ApiResponse();
        try
        {
            var today = DateTime.Today;

            // Lấy statistics của tất cả customers
            var customers = await _unitOfWork.CustomerStatistics
                .GetAllAsync(includes: cs => cs.Customer);

            if (!customers.Any())
            {
                response.IsSuccess = true;
                response.ErrorMessages.Add("Không có khách hàng nào để cập nhật");
                return response;
            }

            // Tạo leaderboard entries cho các loại khác nhau
            var leaderboardTypes = new[]
            {
                LeaderboardType.CurrentStreak,
                LeaderboardType.SmokeFreesDays,
                LeaderboardType.MoneySaved,
                LeaderboardType.CigarettesAvoided
            };

            foreach (var type in leaderboardTypes)
            {
                await UpdateLeaderboardByType(customers, LeaderboardPeriod.Daily, type, today, today);
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.ErrorMessages.Add("Cập nhật bảng xếp hạng hàng ngày thành công");

            _logger.LogInformation("Successfully updated daily leaderboards");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating daily leaderboard");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi cập nhật bảng xếp hạng hàng ngày: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> UpdateWeeklyLeaderboardAsync()
    {
        var response = new ApiResponse();
        try
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekEnd = weekStart.AddDays(6);

            var customers = await _unitOfWork.CustomerStatistics
                .GetAllAsync(includes: cs => cs.Customer);

            if (!customers.Any())
            {
                response.IsSuccess = true;
                response.ErrorMessages.Add("Không có khách hàng nào để cập nhật");
                return response;
            }

            var leaderboardTypes = new[]
            {
                LeaderboardType.CurrentStreak,
                LeaderboardType.SmokeFreesDays,
                LeaderboardType.MoneySaved,
                LeaderboardType.CigarettesAvoided
            };

            foreach (var type in leaderboardTypes)
            {
                await UpdateLeaderboardByType(customers, LeaderboardPeriod.Weekly, type, weekStart, weekEnd);
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.ErrorMessages.Add("Cập nhật bảng xếp hạng hàng tuần thành công");

            _logger.LogInformation("Updated weekly leaderboard for week {WeekStart} - {WeekEnd}",
                weekStart.ToString("yyyy-MM-dd"), weekEnd.ToString("yyyy-MM-dd"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating weekly leaderboard");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi cập nhật bảng xếp hạng hàng tuần: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> UpdateMonthlyLeaderboardAsync()
    {
        var response = new ApiResponse();
        try
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var customers = await _unitOfWork.CustomerStatistics
                .GetAllAsync(includes: cs => cs.Customer);

            if (!customers.Any())
            {
                response.IsSuccess = true;
                response.ErrorMessages.Add("Không có khách hàng nào để cập nhật");
                return response;
            }

            var leaderboardTypes = new[]
            {
                LeaderboardType.CurrentStreak,
                LeaderboardType.SmokeFreesDays,
                LeaderboardType.MoneySaved,
                LeaderboardType.CigarettesAvoided
            };

            foreach (var type in leaderboardTypes)
            {
                await UpdateLeaderboardByType(customers, LeaderboardPeriod.Monthly, type, monthStart, monthEnd);
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.ErrorMessages.Add("Cập nhật bảng xếp hạng hàng tháng thành công");

            _logger.LogInformation("Updated monthly leaderboard for month {Month}/{Year}",
                monthStart.Month, monthStart.Year);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating monthly leaderboard");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi cập nhật bảng xếp hạng hàng tháng: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> GetLeaderboardAsync(string period, string type, int limit = 100)
    {
        var response = new ApiResponse();
        try
        {
            // Parse enum từ string
            if (!Enum.TryParse<LeaderboardPeriod>(period, true, out var leaderboardPeriod))
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Chu kỳ không hợp lệ. Sử dụng: Daily, Weekly, Monthly");
                return response;
            }

            if (!Enum.TryParse<LeaderboardType>(type, true, out var leaderboardType))
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Loại bảng xếp hạng không hợp lệ");
                return response;
            }

            // Lấy dữ liệu leaderboard
            var leaderboards = await _unitOfWork.Leaderboards
                .GetAllAsync(
                    l => l.Period == leaderboardPeriod &&
                         l.Type == leaderboardType &&
                         l.IsActive,
                    l => l.Customer,
                    l => l.Customer.User
                );

            var result = leaderboards
                .OrderBy(l => l.Rank)
                .Take(limit)
                .Select(l => new
                {
                    Rank = l.Rank,
                    CustomerId = l.CustomerId,
                    CustomerName = l.Customer?.FullName ?? "Unknown",
                    Score = l.Score,
                    Type = l.Type.ToString(),
                    Period = l.Period.ToString(),
                    PeriodStart = l.PeriodStart,
                    PeriodEnd = l.PeriodEnd,
                    LastUpdated = l.LastUpdated
                })
                .ToList();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            response.ErrorMessages.Add($"Lấy bảng xếp hạng {type} {period} thành công");

            _logger.LogInformation("Retrieved leaderboard: {Type} {Period} with {Count} entries",
                type, period, result.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leaderboard {Type} {Period}", type, period);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi lấy bảng xếp hạng: {ex.Message}");
        }

        return response;
    }

    private async Task UpdateLeaderboardByType(
        IEnumerable<CustomerStatistics> customers,
        LeaderboardPeriod period,
        LeaderboardType type,
        DateTime periodStart,
        DateTime periodEnd)
    {
        // Sắp xếp customers theo loại leaderboard
        var sortedCustomers = type switch
        {
            LeaderboardType.CurrentStreak => customers.OrderByDescending(c => c.CurrentStreak),
            LeaderboardType.SmokeFreesDays => customers.OrderByDescending(c => c.TotalSmokeFreesDays),
            LeaderboardType.MoneySaved => customers.OrderByDescending(c => c.TotalMoneySaved),
            LeaderboardType.CigarettesAvoided => customers.OrderByDescending(c => c.TotalCigarettesAvoided),
            LeaderboardType.LongestStreak => customers.OrderByDescending(c => c.LongestStreak),
            _ => customers.OrderByDescending(c => c.CurrentStreak)
        };

        // Xóa leaderboard cũ cho period này
        var existingEntries = await _unitOfWork.Leaderboards.GetAllAsync(l =>
            l.Period == period &&
            l.Type == type &&
            l.PeriodStart.Date == periodStart.Date);

        if (existingEntries.Any())
        {
            _unitOfWork.Leaderboards.DeleteRange(existingEntries);
        }

        // Tạo leaderboard entries mới
        var leaderboardEntries = sortedCustomers
            .Select((customer, index) => new Leaderboard
            {
                CustomerId = customer.CustomerId,
                Type = type,
                Period = period,
                Score = GetScoreByType(customer, type),
                Rank = index + 1,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                LastUpdated = DateTime.UtcNow,
                IsActive = true
            })
            .Take(100) // Top 100
            .ToList();

        await _unitOfWork.Leaderboards.AddRangeAsync(leaderboardEntries);
        await _unitOfWork.SaveAsync();
    }

    private int GetScoreByType(CustomerStatistics customer, LeaderboardType type)
    {
        return type switch
        {
            LeaderboardType.CurrentStreak => customer.CurrentStreak,
            LeaderboardType.SmokeFreesDays => customer.TotalSmokeFreesDays,
            LeaderboardType.MoneySaved => (int)customer.TotalMoneySaved, // Convert to int for Score
            LeaderboardType.CigarettesAvoided => customer.TotalCigarettesAvoided,
            LeaderboardType.LongestStreak => customer.LongestStreak,
            _ => customer.CurrentStreak
        };
    }
}
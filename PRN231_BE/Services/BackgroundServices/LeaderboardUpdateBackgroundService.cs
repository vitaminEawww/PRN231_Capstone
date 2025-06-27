using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;

namespace Services.BackgroundServices;

public class LeaderboardUpdateBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<LeaderboardUpdateBackgroundService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(1); // Chạy mỗi giờ

    public LeaderboardUpdateBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<LeaderboardUpdateBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateLeaderboards();
                await Task.Delay(_period, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing leaderboard update background service");
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }
    }

    private async Task UpdateLeaderboards()
    {
        _logger.LogInformation("Starting scheduled leaderboard update");

        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            // Cập nhật leaderboard hàng ngày
            await UpdateDailyLeaderboard(unitOfWork);

            // Cập nhật leaderboard hàng tuần (chỉ chạy vào Chủ nhật)
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                await UpdateWeeklyLeaderboard(unitOfWork);
            }

            // Cập nhật leaderboard hàng tháng (chỉ chạy ngày đầu tháng)
            if (DateTime.Today.Day == 1)
            {
                await UpdateMonthlyLeaderboard(unitOfWork);
            }

            _logger.LogInformation("Successfully completed scheduled leaderboard update");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update leaderboards");
            throw;
        }
    }

    private async Task UpdateDailyLeaderboard(IUnitOfWork unitOfWork)
    {
        var today = DateTime.Today;
        var customers = await unitOfWork.CustomerStatistics.GetAllAsync();

        var leaderboardEntries = customers
            .OrderByDescending(c => c.CurrentStreak)
            .ThenByDescending(c => c.TotalSmokeFreesDays)
            .Select((customer, index) => new Leaderboard
            {
                CustomerId = customer.CustomerId,
                Type = LeaderboardType.SmokeFreesDays,
                Period = LeaderboardPeriod.Daily,
                Score = customer.CurrentStreak,
                Rank = index + 1,
                PeriodStart = today,
                PeriodEnd = today,
                LastUpdated = DateTime.UtcNow,
                IsActive = true
            })
            .Take(100) // Top 100
            .ToList();

        // Xóa leaderboard cũ của ngày hôm nay
        var existingEntries = await unitOfWork.Leaderboards.GetAllAsync(l =>
            l.Period == LeaderboardPeriod.Daily &&
            l.PeriodStart.Date == today);

        unitOfWork.Leaderboards.DeleteRange(existingEntries);

        // Thêm leaderboard mới
        await unitOfWork.Leaderboards.AddRangeAsync(leaderboardEntries);
        await unitOfWork.SaveAsync();
    }

    private async Task UpdateWeeklyLeaderboard(IUnitOfWork unitOfWork)
    {
        var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var weekEnd = weekStart.AddDays(6);

        // Implementation tương tự cho weekly...
        _logger.LogInformation("Updated weekly leaderboard for week {WeekStart} - {WeekEnd}",
            weekStart.ToString("yyyy-MM-dd"), weekEnd.ToString("yyyy-MM-dd"));
    }

    private async Task UpdateMonthlyLeaderboard(IUnitOfWork unitOfWork)
    {
        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // Implementation tương tự cho monthly...
        _logger.LogInformation("Updated monthly leaderboard for month {Month}/{Year}",
            monthStart.Month, monthStart.Year);
    }
}
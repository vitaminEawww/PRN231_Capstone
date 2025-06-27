using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.IServices;

namespace Services.BackgroundServices;

public class StatisticsUpdateBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<StatisticsUpdateBackgroundService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(6); // Chạy mỗi 6 tiếng

    public StatisticsUpdateBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<StatisticsUpdateBackgroundService> logger)
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
                await UpdateAllCustomerStatistics();
                await Task.Delay(_period, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing statistics update background service");

                // Đợi 30 phút trước khi thử lại nếu có lỗi
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }

    private async Task UpdateAllCustomerStatistics()
    {
        _logger.LogInformation("Starting scheduled statistics update for all customers");

        using var scope = _serviceScopeFactory.CreateScope();
        var dailyProgressService = scope.ServiceProvider.GetRequiredService<IDailyProgressService>();

        try
        {
            var result = await dailyProgressService.UpdateAllCustomerStatisticsAsync();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully completed scheduled statistics update");
            }
            else
            {
                _logger.LogWarning("Statistics update completed with warnings: {Messages}",
                    string.Join(", ", result.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update customer statistics");
            throw;
        }
    }
}
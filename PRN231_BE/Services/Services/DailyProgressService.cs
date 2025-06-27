using System.Globalization;
using System.Net;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.DailyProgress;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class DailyProgressService : IDailyProgressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DailyProgressService> _logger;

    public DailyProgressService(
        IUnitOfWork unitOfWork,
        ILogger<DailyProgressService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse> CreateDailyProgressAsync(int customerId, DailyProgressCreateDTO dto)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            var existingProgress = await _unitOfWork.DailyProgresses
                .FirstOrDefaultAsync(dp => dp.CustomerId == customerId && dp.Date.Date == dto.Date.Date);

            if (existingProgress != null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.Conflict;
                response.ErrorMessages.Add("Đã có dữ liệu theo dõi cho ngày này");
                return response;
            }

            var isSmokeFree = dto.CigarettesSmoked == 0 || dto.IsSmokeFree;

            var dailyProgress = new DailyProgress
            {
                CustomerId = customerId,
                Date = dto.Date.Date,
                CigarettesSmoked = dto.CigarettesSmoked,
                MoneySpent = dto.MoneySpent,
                CravingLevel = dto.CravingLevel,
                MoodLevel = dto.MoodLevel,
                EnergyLevel = dto.EnergyLevel,
                Notes = dto.Notes,
                Triggers = dto.Triggers,
                IsSmokeFree = isSmokeFree,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.DailyProgresses.AddAsync(dailyProgress);
            await _unitOfWork.SaveAsync();

            await RecalculateCustomerStatisticsAsync(customerId);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            response.Result = MapToResponseDTO(dailyProgress, customer);

            _logger.LogInformation($"Created daily progress for customer {customerId} on {dto.Date.ToString("yyyy-MM-dd")}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating daily progress for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi tạo dữ liệu theo dõi");
        }

        return response;
    }

    public async Task<ApiResponse> GetDailyProgressByIdAsync(int progressId)
    {
        var response = new ApiResponse();
        try
        {
            var progress = await _unitOfWork.DailyProgresses
                .FirstOrDefaultAsync(dp => dp.Id == progressId, dp => dp.Customer);

            if (progress == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy dữ liệu theo dõi");
                return response;
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = MapToResponseDTO(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily progress {ProgressId}", progressId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi lấy dữ liệu theo dõi");
        }

        return response;
    }

    public async Task<ApiResponse> GetDailyProgressByDateAsync(int customerId, DateTime date)
    {
        var response = new ApiResponse();
        try
        {
            var progress = await _unitOfWork.DailyProgresses
                .FirstOrDefaultAsync(dp => dp.CustomerId == customerId && dp.Date.Date == date.Date, dp => dp.Customer);

            if (progress == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không có dữ liệu theo dõi cho ngày này");
                return response;
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = MapToResponseDTO(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily progress for customer {CustomerId} on {Date}",
                customerId, date.ToString("yyyy-MM-dd"));
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi lấy dữ liệu theo dõi");
        }

        return response;
    }

    public async Task<ApiResponse> UpdateDailyProgressAsync(int customerId, DailyProgressUpdateDTO dto)
    {
        var response = new ApiResponse();
        try
        {
            var progress = await _unitOfWork.DailyProgresses
                .FirstOrDefaultAsync(dp => dp.Id == dto.Id && dp.CustomerId == customerId, dp => dp.Customer);

            if (progress == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy dữ liệu theo dõi hoặc không có quyền truy cập");
                return response;
            }

            // Cập nhật thông tin
            progress.CigarettesSmoked = dto.CigarettesSmoked;
            progress.MoneySpent = dto.MoneySpent;
            progress.CravingLevel = dto.CravingLevel;
            progress.MoodLevel = dto.MoodLevel;
            progress.EnergyLevel = dto.EnergyLevel;
            progress.Notes = dto.Notes;
            progress.Triggers = dto.Triggers;
            progress.IsSmokeFree = dto.CigarettesSmoked == 0 || dto.IsSmokeFree;
            progress.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.DailyProgresses.Update(progress);
            await _unitOfWork.SaveAsync();

            // Cập nhật thống kê
            await RecalculateCustomerStatisticsAsync(customerId);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = MapToResponseDTO(progress);

            _logger.LogInformation("Updated daily progress {ProgressId} for customer {CustomerId}",
                dto.Id, customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating daily progress {ProgressId} for customer {CustomerId}",
                dto.Id, customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi cập nhật dữ liệu theo dõi");
        }

        return response;
    }

    public async Task<ApiResponse> DeleteDailyProgressAsync(int customerId, int progressId)
    {
        var response = new ApiResponse();
        try
        {
            var progress = await _unitOfWork.DailyProgresses
                .FirstOrDefaultAsync(dp => dp.Id == progressId && dp.CustomerId == customerId);

            if (progress == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy dữ liệu theo dõi hoặc không có quyền truy cập");
                return response;
            }

            _unitOfWork.DailyProgresses.Delete(progress);
            await _unitOfWork.SaveAsync();

            // Cập nhật lại thống kê
            await RecalculateCustomerStatisticsAsync(customerId);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            _logger.LogInformation("Deleted daily progress {ProgressId} for customer {CustomerId}",
                progressId, customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting daily progress {ProgressId} for customer {CustomerId}",
                progressId, customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi xóa dữ liệu theo dõi");
        }

        return response;
    }

    public async Task<ApiResponse> GetCustomerDailyProgressAsync(int customerId, ProgressGetListDTO request)
    {
        var response = new ApiResponse();
        try
        {
            var query = _unitOfWork.DailyProgresses.AsQueryable(dp => dp.Customer)
                .Where(dp => dp.CustomerId == customerId);

            if (request.StartDate.HasValue)
                query = query.Where(dp => dp.Date >= request.StartDate.Value.Date);

            if (request.EndDate.HasValue)
                query = query.Where(dp => dp.Date <= request.EndDate.Value.Date);

            request.TotalRecord = await query.CountAsync();
            var pageProgresses = await query
            .OrderByDescending(dp => dp.Date)
            .ToPagedList(request.PageNumber, request.PageSize).ToListAsync();

            var result = pageProgresses.Select(dp => MapToResponseDTO(dp, dp.Customer)).ToList();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new PagingDataModel<DailyProgressResponseDTO, PagingDTO>(result, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily progress list for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi lấy danh sách dữ liệu theo dõi");
        }

        return response;
    }

    public async Task<ApiResponse> GetSmokeFreeeDaysAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var response = new ApiResponse();
        try
        {
            var query = _unitOfWork.DailyProgresses.AsQueryable()
                .Where(dp => dp.CustomerId == customerId && dp.IsSmokeFree);

            if (startDate.HasValue)
                query = query.Where(dp => dp.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(dp => dp.Date <= endDate.Value.Date);

            var smokeFreeeDays = await query
                .OrderByDescending(dp => dp.Date)
                .Select(dp => new
                {
                    dp.Date,
                    dp.CravingLevel,
                    dp.MoodLevel,
                    dp.EnergyLevel,
                    dp.Notes
                })
                .ToListAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new
            {
                TotalSmokeFreeeDays = smokeFreeeDays.Count,
                SmokeFreeeDays = smokeFreeeDays
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting smoke-free days for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi lấy danh sách ngày không hút thuốc");
        }

        return response;
    }

    public async Task<ApiResponse> GetSmokingDaysAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var response = new ApiResponse();
        try
        {
            var query = _unitOfWork.DailyProgresses.AsQueryable()
                .Where(dp => dp.CustomerId == customerId && !dp.IsSmokeFree);

            if (startDate.HasValue)
                query = query.Where(dp => dp.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(dp => dp.Date <= endDate.Value.Date);

            var smokingDays = await query
                .OrderByDescending(dp => dp.Date)
                .Select(dp => new
                {
                    dp.Date,
                    dp.CigarettesSmoked,
                    dp.MoneySpent,
                    dp.CravingLevel,
                    dp.Triggers,
                    dp.Notes
                })
                .ToListAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new
            {
                TotalSmokingDays = smokingDays.Count,
                TotalCigarettesSmoked = smokingDays.Sum(d => d.CigarettesSmoked),
                TotalMoneySpent = smokingDays.Sum(d => d.MoneySpent),
                SmokingDays = smokingDays
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting smoking days for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi lấy danh sách ngày có hút thuốc");
        }

        return response;
    }

    public async Task<ApiResponse> GetCurrentStreakAsync(int customerId)
    {
        var response = new ApiResponse();
        try
        {
            var progressList = await _unitOfWork.DailyProgresses
                .GetAllAsync(dp => dp.CustomerId == customerId);

            var progressArray = progressList.OrderBy(p => p.Date).ToArray();
            var currentStreak = CalculateCurrentStreak(progressArray);
            var longestStreak = CalculateLongestStreak(progressArray);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new
            {
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                LastSmokingDate = progressArray.LastOrDefault(p => !p.IsSmokeFree)?.Date
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current streak for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi tính toán chuỗi ngày");
        }

        return response;
    }

    public async Task<ApiResponse> GetWeeklyProgressSummaryAsync(int customerId, DateTime? weekStartDate = null)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            // Tính toán tuần hiện tại nếu không có input
            var startOfWeek = weekStartDate ?? GetStartOfWeek(DateTime.Today);
            var endOfWeek = startOfWeek.AddDays(6);

            var weeklyData = await GetWeeklyData(customerId, startOfWeek, endOfWeek);
            var previousWeekData = await GetWeeklyData(customerId, startOfWeek.AddDays(-7), startOfWeek.AddDays(-1));

            var summary = new WeeklyProgressSummaryDTO
            {
                CustomerId = customerId,
                CustomerName = customer.FullName,
                WeekStartDate = startOfWeek,
                WeekEndDate = endOfWeek,
                WeekLabel = $"{startOfWeek:dd/MM} - {endOfWeek:dd/MM/yyyy}",
                SmokeFreeeDays = weeklyData.Count(d => d.IsSmokeFree),
                SmokingDays = weeklyData.Count(d => !d.IsSmokeFree),
                TotalCigarettesSmoked = weeklyData.Sum(d => d.CigarettesSmoked),
                TotalMoneySpent = weeklyData.Sum(d => d.MoneySpent),
                AverageCravingLevel = weeklyData.Any() ? (float)weeklyData.Average(d => d.CravingLevel) : 0,
                AverageMoodLevel = weeklyData.Any() ? (float)weeklyData.Average(d => d.MoodLevel) : 0,
                AverageEnergyLevel = weeklyData.Any() ? (float)weeklyData.Average(d => d.EnergyLevel) : 0,
                DailyDetails = weeklyData.Select(d => new DailyProgressSummaryDTO
                {
                    Date = d.Date,
                    IsSmokeFree = d.IsSmokeFree,
                    CigarettesSmoked = d.CigarettesSmoked,
                    MoneySpent = d.MoneySpent,
                    CravingLevel = d.CravingLevel,
                    MoodLevel = d.MoodLevel,
                    EnergyLevel = d.EnergyLevel
                }).ToList()
            };

            // Tính toán so sánh với tuần trước
            summary.Comparison = new WeekComparison
            {
                SmokeFreeeDaysChange = summary.SmokeFreeeDays - previousWeekData.Count(d => d.IsSmokeFree),
                CigarettesChange = summary.TotalCigarettesSmoked - previousWeekData.Sum(d => d.CigarettesSmoked),
                MoneySpentChange = summary.TotalMoneySpent - previousWeekData.Sum(d => d.MoneySpent),
                CravingLevelChange = summary.AverageCravingLevel - (previousWeekData.Any() ? (float)previousWeekData.Average(d => d.CravingLevel) : 0),
                MoodLevelChange = summary.AverageMoodLevel - (previousWeekData.Any() ? (float)previousWeekData.Average(d => d.MoodLevel) : 0),
                EnergyLevelChange = summary.AverageEnergyLevel - (previousWeekData.Any() ? (float)previousWeekData.Average(d => d.EnergyLevel) : 0)
            };

            // Tạo achievements
            summary.Achievements = GenerateWeeklyAchievements(summary);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weekly summary for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi tạo tóm tắt tuần");
        }
        return response;
    }

    public async Task<ApiResponse> GetMonthlyProgressSummaryAsync(int customerId, int? year = null, int? month = null)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            var targetYear = year ?? DateTime.Today.Year;
            var targetMonth = month ?? DateTime.Today.Month;

            var startOfMonth = new DateTime(targetYear, targetMonth, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var monthlyData = await GetMonthlyData(customerId, startOfMonth, endOfMonth);
            var smokingRecord = await _unitOfWork.SmokingRecords.FirstOrDefaultAsync(sr => sr.CustomerId == customerId);

            var summary = new MonthlyProgressSummaryDTO
            {
                CustomerId = customerId,
                CustomerName = customer.FullName,
                Year = targetYear,
                Month = targetMonth,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(targetMonth),
                DaysInMonth = DateTime.DaysInMonth(targetYear, targetMonth),
                SmokeFreeeDaysCount = monthlyData.Count(d => d.IsSmokeFree),
                SmokingDaysCount = monthlyData.Count(d => !d.IsSmokeFree),
                TotalCigarettesSmoked = monthlyData.Sum(d => d.CigarettesSmoked),
                TotalMoneySpent = monthlyData.Sum(d => d.MoneySpent)
            };

            summary.SmokeFreePercentage = summary.DaysInMonth > 0 ? (double)summary.SmokeFreeeDaysCount / summary.DaysInMonth * 100 : 0;

            // Calculate money saved and cigarettes avoided
            if (smokingRecord != null)
            {
                var avgDailyCost = (decimal)smokingRecord.CigarettesPerDay / smokingRecord.CigarettesPerPack * smokingRecord.CostPerPack;
                summary.MoneySaved = summary.SmokeFreeeDaysCount * avgDailyCost;
                summary.CigarettesAvoided = summary.SmokeFreeeDaysCount * smokingRecord.CigarettesPerDay - summary.TotalCigarettesSmoked;
            }

            // Health averages
            if (monthlyData.Any())
            {
                summary.AverageCravingLevel = (float)monthlyData.Average(d => d.CravingLevel);
                summary.AverageMoodLevel = (float)monthlyData.Average(d => d.MoodLevel);
                summary.AverageEnergyLevel = (float)monthlyData.Average(d => d.EnergyLevel);
            }

            // Calculate streaks
            var orderedData = monthlyData.OrderBy(d => d.Date).ToArray();
            summary.BestStreak = CalculateLongestStreak(orderedData);
            summary.CurrentMonthStreak = CalculateCurrentStreak(orderedData);

            // Generate achievements
            summary.MonthlyAchievements = GenerateMonthlyAchievements(summary);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monthly summary for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi tạo tóm tắt tháng");
        }

        return response;
    }

    public async Task<ApiResponse> GetHealthTrendsAsync(int customerId, int days = 30)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-days + 1);

            var progressData = await _unitOfWork.DailyProgresses
                .GetAllAsync(dp => dp.CustomerId == customerId && dp.Date >= startDate && dp.Date <= endDate);

            var orderedData = progressData.OrderBy(p => p.Date).ToArray();

            // Split data into two halves for trend comparison
            var halfPoint = orderedData.Length / 2;
            var firstHalf = orderedData.Take(halfPoint).ToArray();
            var secondHalf = orderedData.Skip(halfPoint).ToArray();

            var trends = new HealthTrendsDTO
            {
                CustomerId = customerId,
                CustomerName = customer.FullName,
                Days = days,
                StartDate = startDate,
                EndDate = endDate,
                DailyMetrics = orderedData.Select(d => new DailyHealthMetric
                {
                    Date = d.Date,
                    CravingLevel = d.CravingLevel,
                    MoodLevel = d.MoodLevel,
                    EnergyLevel = d.EnergyLevel,
                    IsSmokeFree = d.IsSmokeFree
                }).ToList()
            };

            // Calculate trends
            if (firstHalf.Any() && secondHalf.Any())
            {
                trends.CravingTrend = CalculateHealthTrend(firstHalf, secondHalf, d => d.CravingLevel, true);
                trends.MoodTrend = CalculateHealthTrend(firstHalf, secondHalf, d => d.MoodLevel, false);
                trends.EnergyTrend = CalculateHealthTrend(firstHalf, secondHalf, d => d.EnergyLevel, false);
            }

            // Generate insights and recommendations
            trends.Insights = GenerateHealthInsights(orderedData);
            trends.Recommendations = GenerateHealthRecommendations(orderedData);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting health trends for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi phân tích xu hướng sức khỏe");
        }

        return response;
    }

    public async Task<ApiResponse> GetCravingAnalysisAsync(int customerId, int days = 30)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-days + 1);

            var progressData = await _unitOfWork.DailyProgresses
                .GetAllAsync(dp => dp.CustomerId == customerId && dp.Date >= startDate && dp.Date <= endDate);

            var analysis = new CravingAnalysisDTO
            {
                CustomerId = customerId,
                CustomerName = customer.FullName,
                Days = days,
                AnalysisPeriod = DateTime.UtcNow,
                AverageCravingLevel = progressData.Any() ? (float)progressData.Average(p => p.CravingLevel) : 0,
                HighCravingDays = progressData.Count(p => p.CravingLevel >= 7),
                ModerateCravingDays = progressData.Count(p => p.CravingLevel >= 4 && p.CravingLevel <= 6),
                LowCravingDays = progressData.Count(p => p.CravingLevel >= 1 && p.CravingLevel <= 3),
                NoCravingDays = progressData.Count(p => p.CravingLevel == 0)
            };

            // Analyze triggers
            var triggers = progressData
                .Where(p => !string.IsNullOrEmpty(p.Triggers))
                .SelectMany(p => p.Triggers!.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
                .Select(g => new TriggerFrequency
                {
                    Trigger = g.Key,
                    Count = g.Count(),
                    AverageCravingLevel = progressData
                        .Where(p => !string.IsNullOrEmpty(p.Triggers) && p.Triggers.Contains(g.Key, StringComparison.OrdinalIgnoreCase))
                        .Average(p => p.CravingLevel)
                })
                .OrderByDescending(t => t.Count)
                .Take(10)
                .ToList();

            analysis.TopTriggers = triggers;

            // Calculate correlation between craving and smoking
            var smokingData = progressData.Where(p => !p.IsSmokeFree).ToArray();
            if (smokingData.Any())
            {
                analysis.CravingSmokingCorrelation = CalculateCorrelation(
                    smokingData.Select(p => (double)p.CravingLevel).ToArray(),
                    smokingData.Select(p => (double)p.CigarettesSmoked).ToArray()
                );

                analysis.CorrelationDescription = analysis.CravingSmokingCorrelation switch
                {
                    > 0.7f => "Có mối tương quan mạnh giữa mức độ thèm muốn và số điếu hút",
                    > 0.3f => "Có mối tương quan trung bình giữa mức độ thèm muốn và số điếu hút",
                    > -0.3f => "Không có mối tương quan rõ ràng",
                    _ => "Có mối tương quan nghịch giữa mức độ thèm muốn và số điếu hút"
                };
            }

            // Generate recommendations
            analysis.Recommendations = GenerateCravingRecommendations(analysis, progressData.ToArray());

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing craving for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi phân tích mức độ thèm muốn");
        }

        return response;
    }

    public async Task<ApiResponse> GetTriggersAnalysisAsync(int customerId, int days = 30)
    {
        var response = new ApiResponse();
        try
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-days + 1);

            var progressData = await _unitOfWork.DailyProgresses
                .GetAllAsync(dp => dp.CustomerId == customerId && dp.Date >= startDate && dp.Date <= endDate);

            // Phân tích triggers
            var triggerAnalysis = progressData
                .Where(p => !string.IsNullOrEmpty(p.Triggers))
                .SelectMany(p => p.Triggers!.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => new { Trigger = t.Trim(), Progress = p }))
                .GroupBy(x => x.Trigger, StringComparer.OrdinalIgnoreCase)
                .Select(g => new
                {
                    Trigger = g.Key,
                    Count = g.Count(),
                    AverageCravingLevel = g.Average(x => x.Progress.CravingLevel),
                    SmokingRate = g.Count(x => !x.Progress.IsSmokeFree) / (double)g.Count() * 100,
                    DatesOccurred = g.Select(x => x.Progress.Date).OrderByDescending(d => d).Take(5).ToList()
                })
                .OrderByDescending(t => t.Count)
                .ToList();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new
            {
                AnalysisPeriod = $"{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}",
                TotalTriggersRecorded = triggerAnalysis.Sum(t => t.Count),
                UniqueTriggersCount = triggerAnalysis.Count,
                TopTriggers = triggerAnalysis.Take(10),
                TriggerPatterns = new
                {
                    HighRiskTriggers = triggerAnalysis.Where(t => t.SmokingRate > 70).Take(5),
                    FrequentTriggers = triggerAnalysis.Where(t => t.Count >= 3).Take(5),
                    HighCravingTriggers = triggerAnalysis.Where(t => t.AverageCravingLevel >= 7).Take(5)
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting triggers analysis for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi phân tích tác nhân kích hoạt");
        }

        return response;
    }

    public async Task<ApiResponse> GetDailyProgressStatisticsAsync(int customerId)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId, c => c.Statistics, c => c.SmokingRecord);

            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            var progressList = await _unitOfWork.DailyProgresses
                .GetAllAsync(dp => dp.CustomerId == customerId);

            var statistics = await CalculateStatistics(customerId, progressList, customer.SmokingRecord);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi tính toán thống kê");
        }

        return response;
    }

    public async Task<ApiResponse> RecalculateCustomerStatisticsAsync(int customerId)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId, c => c.Statistics, c => c.SmokingRecord);

            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            var progressList = await _unitOfWork.DailyProgresses
                .GetAllAsync(dp => dp.CustomerId == customerId);

            var stats = customer.Statistics ?? new CustomerStatistics { CustomerId = customerId };
            await UpdateCustomerStatistics(stats, progressList, customer.SmokingRecord);

            if (customer.Statistics == null)
            {
                await _unitOfWork.CustomerStatistics.AddAsync(stats);
            }
            else
            {
                _unitOfWork.CustomerStatistics.Update(stats);
            }

            await _unitOfWork.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            _logger.LogInformation("Recalculated statistics for customer {CustomerId}", customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalculating statistics for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi cập nhật thống kê");
        }

        return response;
    }

    public async Task<ApiResponse> UpdateAllCustomerStatisticsAsync()
    {
        var response = new ApiResponse();
        try
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var updateCount = 0;

            foreach (var customer in customers)
            {
                try
                {
                    await RecalculateCustomerStatisticsAsync(customer.Id);
                    updateCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to update statistics for customer {CustomerId}", customer.Id);
                }
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new { UpdatedCustomers = updateCount, TotalCustomers = customers.Count() };

            _logger.LogInformation("Updated statistics for {UpdateCount} customers", updateCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating all customer statistics");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Có lỗi xảy ra khi cập nhật thống kê tất cả khách hàng");
        }

        return response;
    }

    #region Private Helper Methods

    private DailyProgressResponseDTO MapToResponseDTO(DailyProgress progress, Customer? customer = null)
    {
        return new DailyProgressResponseDTO
        {
            Id = progress.Id,
            CustomerId = progress.CustomerId,
            CustomerName = customer?.FullName ?? progress.Customer?.FullName ?? "",
            Date = progress.Date,
            CigarettesSmoked = progress.CigarettesSmoked,
            MoneySpent = progress.MoneySpent,
            CravingLevel = progress.CravingLevel,
            MoodLevel = progress.MoodLevel,
            EnergyLevel = progress.EnergyLevel,
            Notes = progress.Notes,
            Triggers = progress.Triggers,
            IsSmokeFree = progress.IsSmokeFree,
            CreatedAt = progress.CreatedAt,
            UpdatedAt = progress.UpdatedAt
        };
    }

    private async Task<DailyProgressStatisticsDTO> CalculateStatistics(int customerId, IEnumerable<DailyProgress> progressList, SmokingRecord? smokingRecord)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
        var progressArray = progressList.OrderBy(p => p.Date).ToArray();

        // Tính toán các thống kê cơ bản
        var totalDaysTracked = progressArray.Length;
        var smokeFreeeDays = progressArray.Count(p => p.IsSmokeFree);
        var smokingDays = totalDaysTracked - smokeFreeeDays;

        var totalCigarettesSmoked = progressArray.Sum(p => p.CigarettesSmoked);
        var totalMoneySpent = progressArray.Sum(p => p.MoneySpent);

        // Tính streak hiện tại và dài nhất
        var currentStreak = CalculateCurrentStreak(progressArray);
        var longestStreak = CalculateLongestStreak(progressArray);

        // Tính tiền tiết kiệm (dựa trên smoking record)
        var avgDailyCost = smokingRecord != null ?
            (decimal)smokingRecord.CigarettesPerDay / smokingRecord.CigarettesPerPack * smokingRecord.CostPerPack : 0;
        var totalMoneySaved = smokeFreeeDays * avgDailyCost;
        var avgDailySaving = totalDaysTracked > 0 ? totalMoneySaved / totalDaysTracked : 0;

        // Tính điếu thuốc tránh được
        var expectedCigarettes = smokingRecord?.CigarettesPerDay * totalDaysTracked ?? 0;
        var totalCigarettesAvoided = Math.Max(0, expectedCigarettes - totalCigarettesSmoked);

        // Tính trung bình sức khỏe
        var avgCraving = totalDaysTracked > 0 ? (float)progressArray.Average(p => p.CravingLevel) : 0;
        var avgMood = totalDaysTracked > 0 ? (float)progressArray.Average(p => p.MoodLevel) : 0;
        var avgEnergy = totalDaysTracked > 0 ? (float)progressArray.Average(p => p.EnergyLevel) : 0;

        // Tính xu hướng (so với 7 ngày trước)
        var recentProgress = progressArray.TakeLast(7).ToArray();
        var olderProgress = progressArray.SkipLast(7).TakeLast(7).ToArray();

        var cravingTrend = olderProgress.Length > 0 ?
            (float)(recentProgress.Average(p => p.CravingLevel) - olderProgress.Average(p => p.CravingLevel)) : 0;
        var moodTrend = olderProgress.Length > 0 ?
            (float)(recentProgress.Average(p => p.MoodLevel) - olderProgress.Average(p => p.MoodLevel)) : 0;
        var energyTrend = olderProgress.Length > 0 ?
            (float)(recentProgress.Average(p => p.EnergyLevel) - olderProgress.Average(p => p.EnergyLevel)) : 0;

        return new DailyProgressStatisticsDTO
        {
            CustomerId = customerId,
            CustomerName = customer?.FullName ?? "",
            TotalDaysTracked = totalDaysTracked,
            SmokeFreeeDays = smokeFreeeDays,
            SmokingDays = smokingDays,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            LastSmokingDate = progressArray.LastOrDefault(p => !p.IsSmokeFree)?.Date,
            TotalCigarettesSmoked = totalCigarettesSmoked,
            TotalCigarettesAvoided = totalCigarettesAvoided,
            TotalMoneySpent = totalMoneySpent,
            TotalMoneySaved = totalMoneySaved,
            AverageDailyCost = avgDailyCost,
            AverageDailySaving = avgDailySaving,
            AverageCravingLevel = avgCraving,
            AverageMoodLevel = avgMood,
            AverageEnergyLevel = avgEnergy,
            CravingTrend = cravingTrend,
            MoodTrend = moodTrend,
            EnergyTrend = energyTrend,
            LastCalculated = DateTime.UtcNow,
            RecentProgress = progressArray.TakeLast(30).Select(p => new DailyProgressSummaryDTO
            {
                Date = p.Date,
                IsSmokeFree = p.IsSmokeFree,
                CigarettesSmoked = p.CigarettesSmoked,
                MoneySpent = p.MoneySpent,
                CravingLevel = p.CravingLevel,
                MoodLevel = p.MoodLevel,
                EnergyLevel = p.EnergyLevel
            }).ToList()
        };
    }

    private async Task UpdateCustomerStatistics(CustomerStatistics stats, IEnumerable<DailyProgress> progressList, SmokingRecord? smokingRecord)
    {
        var progressArray = progressList.OrderBy(p => p.Date).ToArray();

        // Cập nhật thống kê cơ bản
        stats.TotalSmokeFreesDays = progressArray.Count(p => p.IsSmokeFree);
        stats.CurrentStreak = CalculateCurrentStreak(progressArray);
        stats.LongestStreak = CalculateLongestStreak(progressArray);
        stats.LastSmokingDate = progressArray.LastOrDefault(p => !p.IsSmokeFree)?.Date;

        // Cập nhật thống kê tiền bạc
        var totalDaysTracked = progressArray.Length;
        var avgDailyCost = smokingRecord != null ?
            (decimal)smokingRecord.CigarettesPerDay / smokingRecord.CigarettesPerPack * smokingRecord.CostPerPack : 0;
        stats.TotalMoneySaved = stats.TotalSmokeFreesDays * avgDailyCost;
        stats.AverageDailySaving = totalDaysTracked > 0 ? stats.TotalMoneySaved / totalDaysTracked : 0;

        // Cập nhật thống kê thuốc lá
        var totalCigarettesSmoked = progressArray.Sum(p => p.CigarettesSmoked);
        var expectedCigarettes = smokingRecord?.CigarettesPerDay * totalDaysTracked ?? 0;
        stats.TotalCigarettesAvoided = Math.Max(0, expectedCigarettes - totalCigarettesSmoked);
        stats.TotalPacksAvoided = smokingRecord?.CigarettesPerPack > 0 ?
            stats.TotalCigarettesAvoided / smokingRecord.CigarettesPerPack : 0;

        // Cập nhật thống kê sức khỏe
        if (totalDaysTracked > 0)
        {
            stats.AverageCravingLevel = (float)progressArray.Average(p => p.CravingLevel);
            stats.AverageMoodLevel = (float)progressArray.Average(p => p.MoodLevel);
            stats.AverageEnergyLevel = (float)progressArray.Average(p => p.EnergyLevel);
        }

        stats.LastCalculated = DateTime.UtcNow;
        if (stats.Id == 0)
        {
            stats.CreatedAt = DateTime.UtcNow;
        }
    }

    private int CalculateCurrentStreak(DailyProgress[] progressArray)
    {
        if (progressArray.Length == 0) return 0;

        var streak = 0;
        for (int i = progressArray.Length - 1; i >= 0; i--)
        {
            if (progressArray[i].IsSmokeFree)
                streak++;
            else
                break;
        }
        return streak;
    }

    private int CalculateLongestStreak(DailyProgress[] progressArray)
    {
        if (progressArray.Length == 0) return 0;

        var maxStreak = 0;
        var currentStreak = 0;

        foreach (var progress in progressArray)
        {
            if (progress.IsSmokeFree)
            {
                currentStreak++;
                maxStreak = Math.Max(maxStreak, currentStreak);
            }
            else
            {
                currentStreak = 0;
            }
        }

        return maxStreak;
    }

    private DateTime GetStartOfWeek(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    private async Task<List<DailyProgress>> GetWeeklyData(int customerId, DateTime startDate, DateTime endDate)
    {
        return (await _unitOfWork.DailyProgresses
            .GetAllAsync(dp => dp.CustomerId == customerId && dp.Date >= startDate && dp.Date <= endDate))
            .OrderBy(dp => dp.Date)
            .ToList();
    }

    private async Task<List<DailyProgress>> GetMonthlyData(int customerId, DateTime startDate, DateTime endDate)
    {
        return (await _unitOfWork.DailyProgresses
            .GetAllAsync(dp => dp.CustomerId == customerId && dp.Date >= startDate && dp.Date <= endDate))
            .OrderBy(dp => dp.Date)
            .ToList();
    }

    private List<string> GenerateWeeklyAchievements(WeeklyProgressSummaryDTO summary)
    {
        var achievements = new List<string>();

        if (summary.SmokeFreeeDays == 7)
            achievements.Add("🏆 Tuần hoàn hảo - 7 ngày không hút thuốc!");
        else if (summary.SmokeFreeeDays >= 5)
            achievements.Add($"⭐ Tuần xuất sắc - {summary.SmokeFreeeDays}/7 ngày không hút thuốc!");
        else if (summary.SmokeFreeeDays >= 3)
            achievements.Add($"👍 Tuần tốt - {summary.SmokeFreeeDays}/7 ngày không hút thuốc!");

        if (summary.Comparison.SmokeFreeeDaysChange > 0)
            achievements.Add($"📈 Cải thiện {summary.Comparison.SmokeFreeeDaysChange} ngày so với tuần trước!");

        if (summary.Comparison.CravingLevelChange < -1)
            achievements.Add("😌 Mức độ thèm muốn giảm đáng kể!");

        if (summary.Comparison.MoodLevelChange > 1)
            achievements.Add("😊 Tâm trạng cải thiện rõ rệt!");

        if (summary.TotalMoneySpent == 0)
            achievements.Add($"💰 Tiết kiệm được toàn bộ tiền thuốc trong tuần!");

        return achievements;
    }

    private List<string> GenerateMonthlyAchievements(MonthlyProgressSummaryDTO summary)
    {
        var achievements = new List<string>();

        if (summary.SmokeFreePercentage >= 90)
            achievements.Add("🏆 Tháng vàng - Hơn 90% ngày không hút thuốc!");
        else if (summary.SmokeFreePercentage >= 75)
            achievements.Add("⭐ Tháng bạc - Hơn 75% ngày không hút thuốc!");
        else if (summary.SmokeFreePercentage >= 50)
            achievements.Add("👍 Tháng đồng - Hơn 50% ngày không hút thuốc!");

        if (summary.BestStreak >= 7)
            achievements.Add($"🔥 Chuỗi dài nhất: {summary.BestStreak} ngày!");

        if (summary.MoneySaved > 0)
            achievements.Add($"💰 Tiết kiệm được {summary.MoneySaved:N0} VNĐ!");

        if (summary.CigarettesAvoided > 0)
            achievements.Add($"🚭 Tránh được {summary.CigarettesAvoided} điếu thuốc!");

        return achievements;
    }

    private HealthTrendSummary CalculateHealthTrend(DailyProgress[] firstHalf, DailyProgress[] secondHalf,
        Func<DailyProgress, int> selector, bool lowerIsBetter)
    {
        var previousAvg = (float)firstHalf.Average(selector);
        var currentAvg = (float)secondHalf.Average(selector);
        var change = currentAvg - previousAvg;
        var changePercent = previousAvg != 0 ? (change / previousAvg * 100).ToString("F1") + "%" : "N/A";

        TrendDirection direction;
        if (Math.Abs(change) < 0.5f)
            direction = TrendDirection.Stable;
        else if (lowerIsBetter)
            direction = change < 0 ? TrendDirection.Improving : TrendDirection.Declining;
        else
            direction = change > 0 ? TrendDirection.Improving : TrendDirection.Declining;

        return new HealthTrendSummary
        {
            PreviousAverage = previousAvg,
            CurrentAverage = currentAvg,
            Change = change,
            ChangePercent = changePercent,
            Direction = direction
        };
    }

    private List<string> GenerateHealthInsights(DailyProgress[] data)
    {
        var insights = new List<string>();

        if (data.Length == 0) return insights;

        var smokeFreePercentage = data.Count(d => d.IsSmokeFree) / (double)data.Length * 100;
        var avgCraving = data.Average(d => d.CravingLevel);
        var avgMood = data.Average(d => d.MoodLevel);
        var avgEnergy = data.Average(d => d.EnergyLevel);

        if (smokeFreePercentage >= 80)
            insights.Add($"Bạn đã không hút thuốc {smokeFreePercentage:F1}% thời gian - thật tuyệt vời!");

        if (avgCraving <= 3)
            insights.Add("Mức độ thèm muốn của bạn đang ở mức thấp, cho thấy quá trình cai thuốc hiệu quả!");

        if (avgMood >= 7)
            insights.Add("Tâm trạng của bạn rất tích cực, điều này giúp ích rất nhiều cho việc cai thuốc!");

        if (avgEnergy >= 7)
            insights.Add("Mức năng lượng cao cho thấy sức khỏe của bạn đang cải thiện!");

        var recentWeek = data.TakeLast(7).ToArray();
        var previousWeek = data.SkipLast(7).TakeLast(7).ToArray();

        if (recentWeek.Length > 0 && previousWeek.Length > 0)
        {
            var recentCraving = recentWeek.Average(d => d.CravingLevel);
            var previousCraving = previousWeek.Average(d => d.CravingLevel);

            if (recentCraving < previousCraving - 1)
                insights.Add("Mức độ thèm muốn đang giảm đáng kể so với tuần trước!");
        }

        return insights;
    }

    private List<string> GenerateHealthRecommendations(DailyProgress[] data)
    {
        var recommendations = new List<string>();

        if (data.Length == 0) return recommendations;

        var avgCraving = data.Average(d => d.CravingLevel);
        var avgMood = data.Average(d => d.MoodLevel);
        var avgEnergy = data.Average(d => d.EnergyLevel);
        var smokingDays = data.Count(d => !d.IsSmokeFree);

        if (avgCraving >= 6)
        {
            recommendations.Add("Hãy thử các kỹ thuật thư giãn như thiền định hoặc hít thở sâu khi cảm thấy thèm thuốc");
            recommendations.Add("Tìm các hoạt động thay thế như nhai kẹo cao su hoặc uống nước");
        }

        if (avgMood <= 4)
        {
            recommendations.Add("Hãy tăng cường các hoạt động giải trí và gặp gỡ bạn bè để cải thiện tâm trạng");
            recommendations.Add("Xem xét tham gia nhóm hỗ trợ cai thuốc để có thêm động lực");
        }

        if (avgEnergy <= 4)
        {
            recommendations.Add("Hãy tăng cường vận động và có chế độ ngủ nghỉ đầy đủ");
            recommendations.Add("Bổ sung vitamin và khoáng chất để phục hồi sức khỏe");
        }

        if (smokingDays > data.Length * 0.3)
        {
            recommendations.Add("Hãy xác định và tránh các tác nhân kích hoạt việc hút thuốc");
            recommendations.Add("Xem xét sử dụng các phương pháp hỗ trợ như kẹo cao su nicotine");
        }

        return recommendations;
    }

    private List<string> GenerateCravingRecommendations(CravingAnalysisDTO analysis, DailyProgress[] data)
    {
        var recommendations = new List<string>();

        if (analysis.AverageCravingLevel >= 6)
        {
            recommendations.Add("Mức độ thèm muốn cao - hãy áp dụng kỹ thuật 4-7-8: hít vào 4 giây, nzadržet 7 giây, thở ra 8 giây");
            recommendations.Add("Hãy chuẩn bị sẵn danh sách các hoạt động thay thế khi cảm thấy thèm thuốc");
        }

        if (analysis.TopTriggers.Any())
        {
            var topTrigger = analysis.TopTriggers.First();
            recommendations.Add($"Tác nhân '{topTrigger.Trigger}' xuất hiện thường xuyên nhất - hãy lập kế hoạch để đối phó");
        }

        if (analysis.CravingSmokingCorrelation > 0.5f)
        {
            recommendations.Add("Có mối liên hệ mạnh giữa mức độ thèm muốn và việc hút thuốc - hãy tập trung kiểm soát cảm xúc");
        }

        if (analysis.HighCravingDays > analysis.Days * 0.3)
        {
            recommendations.Add("Số ngày có mức độ thèm muốn cao khá nhiều - hãy xem xét tham khảo ý kiến chuyên gia");
        }

        return recommendations;
    }

    private float CalculateCorrelation(double[] x, double[] y)
    {
        if (x.Length != y.Length || x.Length == 0) return 0;

        var meanX = x.Average();
        var meanY = y.Average();

        var numerator = x.Zip(y, (xi, yi) => (xi - meanX) * (yi - meanY)).Sum();
        var denominator = Math.Sqrt(x.Sum(xi => Math.Pow(xi - meanX, 2)) * y.Sum(yi => Math.Pow(yi - meanY, 2)));

        return denominator == 0 ? 0 : (float)(numerator / denominator);
    }

    #endregion
}
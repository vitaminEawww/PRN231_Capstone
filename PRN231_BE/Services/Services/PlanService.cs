using System.Net;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class PlanService : IPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PlanService> _logger;

    public PlanService(IUnitOfWork unitOfWork, ILogger<PlanService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse> CreateQuitPlanAsync(int customerId, QuitPlanCreateDTO dto)
    {
        var response = new ApiResponse();

        try
        {
            // Kiểm tra customer tồn tại
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông tin khách hàng");
                return response;
            }

            // Kiểm tra ngày hợp lệ
            if (dto.StartDate >= dto.TargetDate)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Ngày bắt đầu phải nhỏ hơn ngày mục tiêu");
                return response;
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Tạo kế hoạch cai thuốc
                var quitPlan = new QuitPlan
                {
                    CustomerId = customerId,
                    Title = dto.Title,
                    Reasons = dto.Reasons,
                    StartDate = dto.StartDate,
                    TargetDate = dto.TargetDate,
                    Status = PlanStatus.Draft,
                    IsSystemGenerated = dto.AutoGeneratePhases,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.QuitPlans.AddAsync(quitPlan);
                await _unitOfWork.SaveAsync();

                // Tạo các giai đoạn
                if (dto.AutoGeneratePhases)
                {
                    await GenerateAutomaticPhasesAsync(quitPlan);
                }
                else if (dto.CustomPhases?.Any() == true)
                {
                    await CreateCustomPhasesAsync(quitPlan.Id, dto.CustomPhases);
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Lấy lại kế hoạch với đầy đủ thông tin
                var createdPlan = await GetQuitPlanWithDetailsAsync(quitPlan.Id);
                response.Result = createdPlan;
                response.StatusCode = HttpStatusCode.Created;
                response.IsSuccess = true;

                _logger.LogInformation($"Tạo kế hoạch cai thuốc thành công cho customer {customerId}");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi tạo kế hoạch cai thuốc");
            _logger.LogError(ex, $"Lỗi tạo kế hoạch cai thuốc cho customer {customerId}");
        }

        return response;
    }

    public async Task<ApiResponse> GetQuitPlanByIdAsync(int planId)
    {
        var response = new ApiResponse();

        try
        {
            var plan = await GetQuitPlanWithDetailsAsync(planId);
            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy kế hoạch cai thuốc");
                return response;
            }
            response.IsSuccess = true;
            response.Result = plan;
            response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi lấy thông tin kế hoạch");
            _logger.LogError(ex, $"Lỗi lấy kế hoạch cai thuốc {planId}");
        }

        return response;
    }

    public async Task<ApiResponse> GetCustomerQuitPlansAsync(int customerId, int pageNumber = 1, int pageSize = 10)
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

            var query = _unitOfWork.QuitPlans.AsQueryable(p => p.Customer, p => p.Phases)
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();
            var plans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var planDtos = plans.Select(MapToResponseDTO).ToList();

            var pagedResult = new PagedList<QuitPlanResponseDTO>(planDtos, totalCount, pageNumber, pageSize);
            response.IsSuccess = true;
            response.Result = pagedResult;
            response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi lấy danh sách kế hoạch");
            _logger.LogError(ex, $"Lỗi lấy danh sách kế hoạch cho customer {customerId}");
        }

        return response;
    }

    public async Task<ApiResponse> UpdateQuitPlanAsync(int customerId, QuitPlanUpdateDTO dto)
    {
        var response = new ApiResponse();

        try
        {
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(p => p.Id == dto.Id && p.CustomerId == customerId);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy kế hoạch cai thuốc");
                return response;
            }

            // Cập nhật các trường nếu có giá trị
            if (!string.IsNullOrEmpty(dto.Title))
                plan.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Reasons))
                plan.Reasons = dto.Reasons;

            if (dto.StartDate.HasValue)
            {
                if (dto.TargetDate.HasValue && dto.StartDate >= dto.TargetDate)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Ngày bắt đầu phải nhỏ hơn ngày mục tiêu");
                    return response;
                }
                plan.StartDate = dto.StartDate.Value;
            }

            if (dto.TargetDate.HasValue)
            {
                if (dto.TargetDate <= plan.StartDate)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Ngày mục tiêu phải lớn hơn ngày bắt đầu");
                    return response;
                }
                plan.TargetDate = dto.TargetDate.Value;
            }

            if (dto.Status.HasValue)
                plan.Status = dto.Status.Value;

            if (dto.Notes != null)
                plan.Notes = dto.Notes;

            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.QuitPlans.Update(plan);
            await _unitOfWork.SaveAsync();

            var updatedPlan = await GetQuitPlanWithDetailsAsync(plan.Id);
            response.Result = updatedPlan;

            _logger.LogInformation($"Cập nhật kế hoạch cai thuốc {plan.Id} thành công");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi cập nhật kế hoạch");
            _logger.LogError(ex, $"Lỗi cập nhật kế hoạch {dto.Id}");
        }

        return response;
    }

    public async Task<ApiResponse> DeleteQuitPlanAsync(int customerId, int planId)
    {
        var response = new ApiResponse();

        try
        {
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.CustomerId == customerId);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy kế hoạch cai thuốc");
                return response;
            }

            // Không cho phép xóa kế hoạch đang active
            if (plan.Status == PlanStatus.Active)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Không thể xóa kế hoạch đang thực hiện. Vui lòng tạm dừng trước khi xóa.");
                return response;
            }

            _unitOfWork.QuitPlans.Delete(plan);
            await _unitOfWork.SaveAsync();

            response.Result = new { PlanId = planId, Message = "Xóa kế hoạch thành công" };
            _logger.LogInformation($"Xóa kế hoạch cai thuốc {planId} thành công");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi xóa kế hoạch");
            _logger.LogError(ex, $"Lỗi xóa kế hoạch {planId}");
        }

        return response;
    }

    public async Task<ApiResponse> GetPlanPhasesAsync(int planId)
    {
        var response = new ApiResponse();

        try
        {
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(p => p.Id == planId, p => p.Phases);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy kế hoạch cai thuốc");
                return response;
            }

            var phases = plan.Phases
                .OrderBy(p => p.PhaseNumber)
                .Select(MapPhaseToResponseDTO)
                .ToList();

            response.Result = phases;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi lấy danh sách giai đoạn");
            _logger.LogError(ex, $"Lỗi lấy danh sách giai đoạn của kế hoạch {planId}");
        }

        return response;
    }

    public async Task<ApiResponse> UpdatePhaseStatusAsync(int customerId, int phaseId, bool isCompleted)
    {
        var response = new ApiResponse();

        try
        {
            // Lấy phase với plan để kiểm tra quyền sở hữu
            var phase = await _unitOfWork.QuitPlans.AsQueryable(qp => qp.Phases)
                .Where(qp => qp.CustomerId == customerId)
                .SelectMany(qp => qp.Phases)
                .FirstOrDefaultAsync(p => p.Id == phaseId);

            if (phase == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy giai đoạn");
                return response;
            }

            phase.IsCompleted = isCompleted;
            phase.CompletedAt = isCompleted ? DateTime.Now : null;

            // Cập nhật phase thông qua repository
            var phaseEntity = await _unitOfWork.QuitPlans.AsQueryable()
                .SelectMany(qp => qp.Phases)
                .FirstOrDefaultAsync(p => p.Id == phaseId);

            if (phaseEntity != null)
            {
                phaseEntity.IsCompleted = isCompleted;
                phaseEntity.CompletedAt = isCompleted ? DateTime.Now : null;
            }

            await _unitOfWork.SaveAsync();

            var updatedPhase = MapPhaseToResponseDTO(phase);
            response.Result = updatedPhase;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            _logger.LogInformation($"Cập nhật trạng thái giai đoạn {phaseId} thành {(isCompleted ? "hoàn thành" : "chưa hoàn thành")}");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi cập nhật trạng thái giai đoạn");
            _logger.LogError(ex, $"Lỗi cập nhật trạng thái giai đoạn {phaseId}");
        }

        return response;
    }

    public async Task<ApiResponse> AddCustomPhaseAsync(int customerId, int planId, QuitPlanPhaseCreateDTO dto)
    {
        var response = new ApiResponse();

        try
        {
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.CustomerId == customerId, p => p.Phases);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy kế hoạch cai thuốc");
                return response;
            }

            // Kiểm tra phase number đã tồn tại
            if (plan.Phases.Any(p => p.PhaseNumber == dto.PhaseNumber))
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add($"Giai đoạn số {dto.PhaseNumber} đã tồn tại");
                return response;
            }

            // Kiểm tra ngày hợp lệ
            if (dto.StartDate >= dto.EndDate)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                return response;
            }

            var newPhase = new QuitPlanPhase
            {
                QuitPlanId = planId,
                PhaseNumber = dto.PhaseNumber,
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TargetCigarettesPerDay = dto.TargetCigarettesPerDay
            };

            plan.Phases.Add(newPhase);
            await _unitOfWork.SaveAsync();

            var createdPhase = MapPhaseToResponseDTO(newPhase);
            response.Result = createdPhase;
            response.StatusCode = HttpStatusCode.Created;
            response.IsSuccess = true;

            _logger.LogInformation($"Thêm giai đoạn mới cho kế hoạch {planId} thành công");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi thêm giai đoạn");
            _logger.LogError(ex, $"Lỗi thêm giai đoạn cho kế hoạch {planId}");
        }

        return response;
    }

    public async Task<ApiResponse> UpdatePhaseAsync(int customerId, int phaseId, QuitPlanPhaseCreateDTO dto)
    {
        var response = new ApiResponse();

        try
        {
            // Lấy phase với plan để kiểm tra quyền sở hữu
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(qp => qp.CustomerId == customerId && qp.Phases.Any(p => p.Id == phaseId),
                    qp => qp.Phases);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy giai đoạn");
                return response;
            }

            var phase = plan.Phases.First(p => p.Id == phaseId);

            // Kiểm tra ngày hợp lệ
            if (dto.StartDate >= dto.EndDate)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                return response;
            }

            // Kiểm tra phase number conflict (nếu thay đổi)
            if (dto.PhaseNumber != phase.PhaseNumber &&
                plan.Phases.Any(p => p.PhaseNumber == dto.PhaseNumber))
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add($"Giai đoạn số {dto.PhaseNumber} đã tồn tại");
                return response;
            }

            // Cập nhật thông tin
            phase.PhaseNumber = dto.PhaseNumber;
            phase.Title = dto.Title;
            phase.Description = dto.Description;
            phase.StartDate = dto.StartDate;
            phase.EndDate = dto.EndDate;
            phase.TargetCigarettesPerDay = dto.TargetCigarettesPerDay;

            await _unitOfWork.SaveAsync();

            var updatedPhase = MapPhaseToResponseDTO(phase);
            response.Result = updatedPhase;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            _logger.LogInformation($"Cập nhật giai đoạn {phaseId} thành công");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi cập nhật giai đoạn");
            _logger.LogError(ex, $"Lỗi cập nhật giai đoạn {phaseId}");
        }

        return response;
    }

    public async Task<ApiResponse> DeletePhaseAsync(int customerId, int phaseId)
    {
        var response = new ApiResponse();

        try
        {
            // Lấy phase với plan để kiểm tra quyền sở hữu
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(qp => qp.CustomerId == customerId && qp.Phases.Any(p => p.Id == phaseId),
                    qp => qp.Phases);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy giai đoạn");
                return response;
            }

            var phase = plan.Phases.First(p => p.Id == phaseId);

            // Không cho phép xóa phase đã hoàn thành
            if (phase.IsCompleted)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Không thể xóa giai đoạn đã hoàn thành");
                return response;
            }

            plan.Phases.Remove(phase);
            await _unitOfWork.SaveAsync();

            response.Result = new { PhaseId = phaseId, Message = "Xóa giai đoạn thành công" };
            _logger.LogInformation($"Xóa giai đoạn {phaseId} thành công");
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi xóa giai đoạn");
            _logger.LogError(ex, $"Lỗi xóa giai đoạn {phaseId}");
        }

        return response;
    }

    public async Task<ApiResponse> GetPlanStatisticsAsync(int customerId, int planId)
    {
        var response = new ApiResponse();

        try
        {
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.CustomerId == customerId, p => p.Phases, p => p.Customer);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy kế hoạch cai thuốc");
                return response;
            }

            var customer = await _unitOfWork.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId, c => c.SmokingRecord);

            var smokingRecord = customer?.SmokingRecord;
            var totalDays = (plan.TargetDate - plan.StartDate).Days;
            var daysElapsed = Math.Max(0, (DateTime.Today - plan.StartDate).Days);
            var daysRemaining = Math.Max(0, (plan.TargetDate - DateTime.Today).Days);
            var progressPercentage = totalDays > 0 ? Math.Min(100, (double)daysElapsed / totalDays * 100) : 0;

            // Tính toán tiết kiệm chi phí
            var dailyCost = smokingRecord != null ?
                (smokingRecord.CostPerPack / smokingRecord.CigarettesPerPack) * smokingRecord.CigarettesPerDay : 0;
            var estimatedSavings = (decimal)(dailyCost * daysElapsed);

            // Thống kê giai đoạn
            var totalPhases = plan.Phases.Count;
            var completedPhases = plan.Phases.Count(p => p.IsCompleted);
            var currentPhase = plan.Phases
                .Where(p => DateTime.Today >= p.StartDate && DateTime.Today <= p.EndDate)
                .FirstOrDefault();

            var statistics = new
            {
                PlanId = planId,
                Title = plan.Title,
                Status = plan.Status.ToString(),
                StatusText = GetStatusText(plan.Status),

                // Thời gian
                StartDate = plan.StartDate,
                TargetDate = plan.TargetDate,
                TotalDays = totalDays,
                DaysElapsed = daysElapsed,
                DaysRemaining = daysRemaining,
                ProgressPercentage = Math.Round(progressPercentage, 2),

                // Giai đoạn
                TotalPhases = totalPhases,
                CompletedPhases = completedPhases,
                CurrentPhase = currentPhase != null ? new
                {
                    Id = currentPhase.Id,
                    PhaseNumber = currentPhase.PhaseNumber,
                    Title = currentPhase.Title,
                    TargetCigarettesPerDay = currentPhase.TargetCigarettesPerDay,
                    DaysRemaining = Math.Max(0, (currentPhase.EndDate - DateTime.Today).Days)
                } : null,
                PhaseProgress = totalPhases > 0 ? Math.Round((double)completedPhases / totalPhases * 100, 2) : 0,

                // Tài chính
                EstimatedSavings = estimatedSavings,
                DailyCostBefore = dailyCost,

                // Sức khỏe (có thể mở rộng sau)
                HealthBenefits = GetHealthBenefits(daysElapsed)
            };

            response.Result = statistics;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi lấy thống kê kế hoạch");
            _logger.LogError(ex, $"Lỗi lấy thống kê kế hoạch {planId}");
        }

        return response;
    }

    public async Task<ApiResponse> StartQuitPlanAsync(int customerId, int planId)
    {
        return await UpdatePlanStatusAsync(customerId, planId, PlanStatus.Active);
    }

    public async Task<ApiResponse> PauseQuitPlanAsync(int customerId, int planId)
    {
        return await UpdatePlanStatusAsync(customerId, planId, PlanStatus.Paused);
    }

    public async Task<ApiResponse> ResumeQuitPlanAsync(int customerId, int planId)
    {
        return await UpdatePlanStatusAsync(customerId, planId, PlanStatus.Active);
    }

    public async Task<ApiResponse> CompleteQuitPlanAsync(int customerId, int planId)
    {
        return await UpdatePlanStatusAsync(customerId, planId, PlanStatus.Completed);
    }

    public async Task<ApiResponse> FailQuitPlanAsync(int customerId, int planId)
    {
        return await UpdatePlanStatusAsync(customerId, planId, PlanStatus.Failed);
    }

    public async Task<ApiResponse> GenerateRecommendedPlanAsync(int customerId)
    {
        var response = new ApiResponse();

        try
        {
            // Lấy thông tin customer và smoking record
            var customer = await _unitOfWork.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId, c => c.SmokingRecord);

            if (customer?.SmokingRecord == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Chưa có thông tin hút thuốc. Vui lòng cập nhật thông tin trước khi tạo kế hoạch");
                return response;
            }

            var smokingRecord = customer.SmokingRecord;
            var currentCigarettesPerDay = smokingRecord.CigarettesPerDay;

            // Tạo kế hoạch được đề xuất
            var recommendedPlan = new QuitPlanCreateDTO
            {
                Title = "Kế hoạch cai thuốc được đề xuất",
                Reasons = "Cải thiện sức khỏe và tiết kiệm chi phí",
                StartDate = DateTime.Today.AddDays(3), // Bắt đầu sau 3 ngày để chuẩn bị
                TargetDate = DateTime.Today.AddDays(90), // Mục tiêu 3 tháng
                AutoGeneratePhases = true,
                Notes = $"Kế hoạch được tạo tự động dựa trên thói quen hút {currentCigarettesPerDay} điếu/ngày"
            };
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Result = recommendedPlan;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi tạo kế hoạch đề xuất");
            _logger.LogError(ex, $"Lỗi tạo kế hoạch đề xuất cho customer {customerId}");
        }

        return response;
    }

    #region Private Helper Method
    private async Task GenerateAutomaticPhasesAsync(QuitPlan quitPlan)
    {
        // Lấy thông tin hút thuốc hiện tại
        var customer = await _unitOfWork.Customers
            .FirstOrDefaultAsync(c => c.Id == quitPlan.CustomerId, c => c.SmokingRecord);

        var currentCigarettes = customer?.SmokingRecord?.CigarettesPerDay ?? 20;
        var totalDays = (quitPlan.TargetDate - quitPlan.StartDate).Days;

        // Tạo 4 giai đoạn giảm dần
        var phaseDuration = totalDays / 4;

        for (int i = 0; i < 4; i++)
        {
            var phaseStart = quitPlan.StartDate.AddDays(i * phaseDuration);
            var phaseEnd = i == 3 ? quitPlan.TargetDate : phaseStart.AddDays(phaseDuration - 1);

            // Giảm dần từ 75% -> 50% -> 25% -> 0%
            var reductionRate = 1.0 - (i + 1) * 0.25;
            var targetCigarettes = (int)(currentCigarettes * reductionRate);

            var phase = new QuitPlanPhase
            {
                QuitPlanId = quitPlan.Id,
                PhaseNumber = i + 1,
                Title = $"Giai đoạn {i + 1}: Giảm xuống {targetCigarettes} điếu/ngày",
                Description = GetPhaseDescription(i + 1, targetCigarettes),
                StartDate = phaseStart,
                EndDate = phaseEnd,
                TargetCigarettesPerDay = targetCigarettes
            };

            quitPlan.Phases.Add(phase);
        }
    }

    private string GetPhaseDescription(int phaseNumber, int targetCigarettes)
    {
        return phaseNumber switch
        {
            1 => $"Giảm từ từ xuống {targetCigarettes} điếu mỗi ngày. Tập trung vào việc xác định các tác nhân kích hoạt và tìm cách thay thế.",
            2 => $"Tiếp tục giảm xuống {targetCigarettes} điếu/ngày. Tăng cường hoạt động thể chất và thư giãn.",
            3 => $"Giai đoạn quan trọng - chỉ còn {targetCigarettes} điếu/ngày. Tập trung vào việc kiểm soát cảm xúc.",
            4 => "Giai đoạn cuối - hoàn toàn ngừng hút thuốc. Duy trì lối sống lành mạnh và tránh xa thuốc lá.",
            _ => $"Mục tiêu {targetCigarettes} điếu/ngày"
        };
    }

    private async Task CreateCustomPhasesAsync(int planId, List<QuitPlanPhaseCreateDTO> customPhases)
    {
        // Lấy plan entity để thao tác trực tiếp
        var plan = await _unitOfWork.QuitPlans.GetByIdAsync(planId);
        if (plan == null) return;

        foreach (var phaseDto in customPhases.OrderBy(p => p.PhaseNumber))
        {
            var phase = new QuitPlanPhase
            {
                QuitPlanId = planId,
                PhaseNumber = phaseDto.PhaseNumber,
                Title = phaseDto.Title,
                Description = phaseDto.Description,
                StartDate = phaseDto.StartDate,
                EndDate = phaseDto.EndDate,
                TargetCigarettesPerDay = phaseDto.TargetCigarettesPerDay
            };

            // Sử dụng collection navigation property
            plan.Phases.Add(phase);
        }
    }

    private async Task<ApiResponse> UpdatePlanStatusAsync(int customerId, int planId, PlanStatus newStatus)
    {
        var response = new ApiResponse();

        try
        {
            var plan = await _unitOfWork.QuitPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.CustomerId == customerId);

            if (plan == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy kế hoạch cai thuốc");
                return response;
            }

            plan.Status = newStatus;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.QuitPlans.Update(plan);
            await _unitOfWork.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new { PlanId = planId, NewStatus = newStatus.ToString() };

            _logger.LogInformation($"Cập nhật trạng thái kế hoạch {planId} thành {newStatus}");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add("Lỗi hệ thống khi cập nhật trạng thái kế hoạch");
            _logger.LogError(ex, $"Lỗi cập nhật trạng thái kế hoạch {planId}");
        }

        return response;
    }

    private async Task<QuitPlanResponseDTO?> GetQuitPlanWithDetailsAsync(int planId)
    {
        var plan = await _unitOfWork.QuitPlans
            .FirstOrDefaultAsync(p => p.Id == planId, p => p.Customer, p => p.Phases);

        return plan == null ? null : MapToResponseDTO(plan);
    }

    private QuitPlanResponseDTO MapToResponseDTO(QuitPlan plan)
    {
        var totalDays = (plan.TargetDate - plan.StartDate).Days;
        var daysRemaining = (plan.TargetDate - DateTime.Today).Days;
        var completedPhases = plan.Phases.Count(p => p.IsCompleted);
        var totalPhases = plan.Phases.Count;

        return new QuitPlanResponseDTO
        {
            Id = plan.Id,
            CustomerId = plan.CustomerId,
            CustomerName = plan.Customer?.FullName ?? "",
            Title = plan.Title,
            Reasons = plan.Reasons,
            StartDate = plan.StartDate,
            TargetDate = plan.TargetDate,
            Status = plan.Status,
            StatusText = GetStatusText(plan.Status),
            IsSystemGenerated = plan.IsSystemGenerated,
            Notes = plan.Notes,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            Phases = plan.Phases.Select(MapPhaseToResponseDTO).OrderBy(p => p.PhaseNumber).ToList(),
            OverallProgress = totalPhases > 0 ? (double)completedPhases / totalPhases * 100 : 0,
            TotalPhases = totalPhases,
            CompletedPhases = completedPhases,
            DaysRemaining = Math.Max(0, daysRemaining),
            TotalDays = totalDays
        };
    }

    private QuitPlanPhaseResponseDTO MapPhaseToResponseDTO(QuitPlanPhase phase)
    {
        var today = DateTime.Today;
        var isActive = today >= phase.StartDate && today <= phase.EndDate && !phase.IsCompleted;
        var isUpcoming = today < phase.StartDate;
        var daysRemaining = (phase.EndDate - today).Days;
        var totalDays = (phase.EndDate - phase.StartDate).Days;

        return new QuitPlanPhaseResponseDTO
        {
            Id = phase.Id,
            QuitPlanId = phase.QuitPlanId,
            PhaseNumber = phase.PhaseNumber,
            Title = phase.Title,
            Description = phase.Description,
            StartDate = phase.StartDate,
            EndDate = phase.EndDate,
            TargetCigarettesPerDay = phase.TargetCigarettesPerDay,
            IsCompleted = phase.IsCompleted,
            CompletedAt = phase.CompletedAt,
            IsActive = isActive,
            IsUpcoming = isUpcoming,
            DaysRemaining = Math.Max(0, daysRemaining),
            TotalDays = totalDays
        };
    }

    private string GetStatusText(PlanStatus status)
    {
        return status switch
        {
            PlanStatus.Draft => "Bản nháp",
            PlanStatus.Active => "Đang thực hiện",
            PlanStatus.Paused => "Tạm dừng",
            PlanStatus.Completed => "Hoàn thành",
            PlanStatus.Failed => "Thất bại",
            _ => "Không xác định"
        };
    }

    private object GetHealthBenefits(int daysElapsed)
    {
        var benefits = new List<string>();

        if (daysElapsed >= 1)
            benefits.Add("Lưu thông máu cải thiện");
        if (daysElapsed >= 3)
            benefits.Add("Hơi thở dễ dàng hơn");
        if (daysElapsed >= 7)
            benefits.Add("Vị giác và khứu giác được cải thiện");
        if (daysElapsed >= 30)
            benefits.Add("Giảm ho và khó thở");
        if (daysElapsed >= 90)
            benefits.Add("Chức năng phổi cải thiện đáng kể");

        return new
        {
            DaysSmokeFree = daysElapsed,
            Benefits = benefits
        };
    }
    #endregion
}
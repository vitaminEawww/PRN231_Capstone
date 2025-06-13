using DataAccess.Common;
using DataAccess.Constants;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.Auth;
using Microsoft.Extensions.Configuration;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly int _maxFailedAttempts = 5;
        private readonly int _lockoutMinutes = 30;

        public AuthService(
            IUnitOfWork unitOfWork,
            JwtTokenService jwtTokenService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        public async Task<ApiResponse> RegisterCustomerAsync(RegisterCustomerRequestDTO request)
        {
            var response = new ApiResponse();

            try
            {
                var validationResult = request.Validate();
                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add(validationResult.ErrorMessage);
                    return response;
                }

                // Kiểm tra email/username đã tồn tại
                var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(
                    u => u.Email == request.Email || u.UserName == request.Username);

                if (existingUser != null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Email hoặc Username đã được sử dụng");
                    return response;
                }

                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Tạo User chung
                    var user = new User
                    {
                        UserName = request.Username,
                        Email = request.Email,
                        Phone = request.Phone,
                        PasswordHash = Helper.HashPassword(request.Password),
                        Role = UserRole.Customer,
                        Status = UserStatus.Active,
                        JoinDate = DateTime.Now,
                        LastLoginAt = DateTime.Now,
                        IsEmailVerified = true,
                        EmailVerificationToken = null,
                        EmailVerifiedAt = DateTime.Now,
                        PasswordResetToken = null,
                        PasswordResetTokenExpiresAt = null,
                        FailedLoginAttempts = 0,
                    };

                    await _unitOfWork.Users.AddAsync(user);
                    await _unitOfWork.SaveAsync();

                    await CreateCustomerProfileAsync(user, request);

                    await transaction.CommitAsync();

                    var authResponse = await CreateAuthResponseAsync(user);

                    response.IsSuccess = true;
                    response.Result = authResponse;

                    return response;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Lỗi đăng ký: {ex.Message}");
                return response;
            }
        }

        public async Task<ApiResponse> RegisterCoachAsync(RegisterCoachRequestDTO request)
        {
            var response = new ApiResponse();

            try
            {
                var validationResult = request.Validate();
                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add(validationResult.ErrorMessage);
                    return response;
                }

                // Kiểm tra email/username đã tồn tại
                var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(
                    u => u.Email == request.Email || u.UserName == request.Username);

                if (existingUser != null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Email hoặc Username đã được sử dụng");
                    return response;
                }

                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Tạo User chung
                    var user = new User
                    {
                        UserName = request.Username,
                        Email = request.Email,
                        Phone = request.Phone,
                        PasswordHash = Helper.HashPassword(request.Password),
                        Role = UserRole.Coach,
                        Status = UserStatus.Active,
                        JoinDate = DateTime.Now,
                        LastLoginAt = DateTime.Now,
                        IsEmailVerified = true,
                        EmailVerificationToken = null,
                        EmailVerifiedAt = DateTime.Now,
                        PasswordResetToken = null,
                        PasswordResetTokenExpiresAt = null,
                        FailedLoginAttempts = 0,
                    };

                    await _unitOfWork.Users.AddAsync(user);
                    await _unitOfWork.SaveAsync();

                    await CreateCoachProfileAsync(user, request);

                    await transaction.CommitAsync();

                    var authResponse = await CreateAuthResponseAsync(user);

                    response.IsSuccess = true;
                    response.Result = authResponse;

                    return response;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Lỗi đăng ký: {ex.Message}");
                return response;
            }
        }

        public async Task<ApiResponse> LoginAsync(LoginRequestDTO request)
        {
            var response = new ApiResponse();
            User? user = null;
            try
            {
                if (request.UserName.Contains("@"))
                {
                    // Xử lý như email
                    user = await _unitOfWork.Users.FirstOrDefaultAsync(
                        u => u.Email == request.UserName
                    );

                    if (user == null)
                    {
                        user = await _unitOfWork.Users.FirstOrDefaultAsync(
                            x => x.Email.Replace(".", "") == request.UserName.Replace(".", "")
                        );
                    }
                }
                else
                {
                    // Xử lý như username
                    user = await _unitOfWork.Users.FirstOrDefaultAsync(
                        u => u.UserName == request.UserName
                    );
                }

                if (user == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Email hoặc UserName không đúng.");
                    return response;
                }

                // Kiểm tra account bị khóa
                if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.Now)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add($"Tài khoản bị khóa đến {user.LockedUntil:dd/MM/yyyy HH:mm}");
                    return response;
                }

                // Kiểm tra status
                if (user.Status == UserStatus.Blocked || user.Status == UserStatus.Deleted)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Tài khoản đã bị vô hiệu hóa");
                    return response;
                }

                // Kiểm tra mật khẩu
                if (!Helper.VerifyPassword(request.Password, user.PasswordHash))
                {
                    // Tăng failed attempts
                    user.FailedLoginAttempts++;

                    if (user.FailedLoginAttempts >= _maxFailedAttempts)
                    {
                        user.LockedUntil = DateTime.UtcNow.AddMinutes(_lockoutMinutes);
                        user.FailedLoginAttempts = 0;
                    }

                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveAsync();

                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Email hoặc mật khẩu không đúng");
                    return response;
                }

                // Reset failed attempts và update last login
                user.FailedLoginAttempts = 0;
                user.LockedUntil = null;
                user.LastLoginAt = DateTime.UtcNow;
                user.Status = UserStatus.Active;

                // Generate access token
                var accessToken = _jwtTokenService.GenerateToken(user);

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();

                // Tạo response
                var authResponse = await CreateAuthResponseAsync(user);

                response.IsSuccess = true;
                response.Result = authResponse;

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Lỗi đăng nhập: {ex.Message}");
                return response;
            }
        }

        public async Task<ApiResponse> GetUserProfileAsync(int userId)
        {
            var response = new ApiResponse();

            try
            {
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(
                    u => u.Id == userId,
                    u => u.Customer!);

                if (user == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Người dùng không tồn tại");
                    return response;
                }

                var userInfo = new UserInfoDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString(),
                    JoinDate = user.JoinDate,
                    Customer = user.Customer != null ? new CustomerInfoDTO
                    {
                        Id = user.Customer.Id,
                        FullName = user.Customer.FullName,
                        AvatarUrl = user.Customer.AvatarUrl,
                        IsNotificationEnabled = user.Customer.IsNotificationEnabled
                    } : null
                };

                response.IsSuccess = true;
                response.Result = userInfo;

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Lỗi lấy thông tin profile: {ex.Message}");
                return response;
            }
        }


        #region Private Helper Methods
        private async Task CreateCustomerProfileAsync(User user, RegisterCustomerRequestDTO request)
        {
            var customer = new Customer
            {
                Id = user.Id,
                FullName = request.FullName,
                Gender = request.Gender!.Value,
                DateOfBirth = request.DateOfBirth,
                AvatarUrl = string.IsNullOrEmpty(request.AvatarUrl)
                    ? ConstantModel.DefaultAvatar
                    : request.AvatarUrl,
                Bio = request.Bio,
                CreatedAt = DateTime.UtcNow,
                IsNotificationEnabled = true,
                IsDailyReminderEnabled = true,
                IsWeeklyReportEnabled = true
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveAsync();
        }

        private async Task CreateCoachProfileAsync(User user, RegisterCoachRequestDTO request)
        {
            var coach = new Coach
            {
                UserId = user.Id,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                AvatarUrl = string.IsNullOrEmpty(request.AvatarUrl)
                    ? ConstantModel.DefaultAvatar
                    : request.AvatarUrl,
                Bio = request.Bio!,
                Specialization = request.Specialization!,
                Qualifications = request.Qualifications!,
                ExperienceYears = request.ExperienceYears!.Value,
                HourlyRate = request.HourlyRate!.Value,
                IsAvailable = true,
                Rating = 0,
                TotalConsultations = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Coaches.AddAsync(coach);
            await _unitOfWork.SaveAsync();
        }

        private async Task<AuthResponseDTO> CreateAuthResponseAsync(User user)
        {
            var token = _jwtTokenService.GenerateToken(user);

            // Load profile data based on role
            CustomerInfoDTO? customerInfo = null;
            CoachInfoDTO? coachInfo = null;

            if (user.Role == UserRole.Customer)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(user.Id);
                if (customer != null)
                {
                    customerInfo = new CustomerInfoDTO
                    {
                        Id = customer.Id,
                        FullName = customer.FullName,
                        AvatarUrl = customer.AvatarUrl,
                        IsNotificationEnabled = customer.IsNotificationEnabled,
                    };
                }
            }
            else if (user.Role == UserRole.Coach)
            {
                var coach = await _unitOfWork.Coaches.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (coach != null)
                {
                    coachInfo = new CoachInfoDTO
                    {
                        Id = coach.Id,
                        FullName = coach.FullName,
                        AvatarUrl = coach.AvatarUrl,
                        Bio = coach.Bio,
                        Specialization = coach.Specialization,
                        ExperienceYears = coach.ExperienceYears,
                        HourlyRate = coach.HourlyRate,
                        IsAvailable = coach.IsAvailable,
                        Rating = coach.Rating,
                        TotalConsultations = coach.TotalConsultations
                    };
                }
            }

            return new AuthResponseDTO
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60")),
                User = new UserInfoDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString(),
                    JoinDate = user.JoinDate,
                    Customer = customerInfo,
                    Coach = coachInfo
                }
            };

        }
        #endregion
    }
}
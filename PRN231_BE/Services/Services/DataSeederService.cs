using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class DataSeederService : IDataSeederService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DataSeederService> _logger;

    public DataSeederService(
        IUnitOfWork unitOfWork,
        ILogger<DataSeederService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedDefaultDataAsync()
    {
        try
        {
            await SeedAdminUserAsync();
            await SeedSampleUsersAsync();

            _logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data seeding");
            throw;
        }
    }


    public async Task SeedAdminUserAsync()
    {
        try
        {
            var adminEmail = "admin@smokefree.com";
            var adminPhone = "0123456789";

            var existingAdmin = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (existingAdmin != null)
            {
                _logger.LogInformation($"Admin user '{adminEmail}' already exists");
                return;
            }

            // Tạo admin user
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                Phone = adminPhone,
                PasswordHash = Helper.HashPassword("Admin@123456"),
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                JoinDate = DateTime.UtcNow,
                IsEmailVerified = true,
                FailedLoginAttempts = 0
            };

            await _unitOfWork.Users.AddAsync(adminUser);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation($"Admin user '{adminEmail}' created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating admin user");
            throw;
        }
    }

    public async Task SeedSampleUsersAsync()
    {
        try
        {
            // Sample customer user
            var customerEmail = "customer@example.com";
            var existingCustomer = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == customerEmail);

            if (existingCustomer == null)
            {
                var customerUser = new User
                {
                    UserName = "customer_demo",
                    Email = customerEmail,
                    Phone = "0987654321",
                    PasswordHash = Helper.HashPassword("Customer@123456"),
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    JoinDate = DateTime.UtcNow,
                    IsEmailVerified = true,
                    FailedLoginAttempts = 0
                };

                await _unitOfWork.Users.AddAsync(customerUser);
                await _unitOfWork.SaveAsync();

                // Tạo customer profile
                var customerProfile = new Customer
                {
                    Id = customerUser.Id, // One-to-One relationship
                    FullName = "Sample Customer",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Bio = "Demo customer account for testing purposes",
                    IsNotificationEnabled = true,
                    IsDailyReminderEnabled = true,
                    IsWeeklyReportEnabled = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Customers.AddAsync(customerProfile);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation($"Sample customer '{customerEmail}' created successfully");
            }

            // Sample coach user  
            var coachEmail = "coach@smokefree.com";
            var existingCoach = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == coachEmail);

            if (existingCoach == null)
            {
                var coachUser = new User
                {
                    UserName = "coach_demo",
                    Email = coachEmail,
                    Phone = "0123456788",
                    PasswordHash = Helper.HashPassword("Coach@123456"),
                    Role = UserRole.Coach,
                    Status = UserStatus.Active,
                    JoinDate = DateTime.UtcNow,
                    IsEmailVerified = true,
                    FailedLoginAttempts = 0
                };

                await _unitOfWork.Users.AddAsync(coachUser);
                await _unitOfWork.SaveAsync();

                // Tạo coach profile
                var coachProfile = new Coach
                {
                    UserId = coachUser.Id,
                    FullName = "Dr. Demo Coach",
                    Bio = "Experienced smoking cessation coach with 10+ years of experience",
                    Specialization = "Behavioral Therapy, Addiction Treatment",
                    ExperienceYears = 10,
                    HourlyRate = 50.00m,
                    IsAvailable = true,
                    Rating = 4.8f,
                    TotalConsultations = 0,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Coaches.AddAsync(coachProfile);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation($"Sample coach '{coachEmail}' created successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sample users");
            throw;
        }
    }
}
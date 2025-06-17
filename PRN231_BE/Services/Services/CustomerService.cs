using System;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> GetCustomerIdByUserId(int userId)
    {
        var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        return customer?.Id ?? 0;
    }
}

using System;

namespace Services.IServices;

public interface ICustomerService
{
    Task<int> GetCustomerIdByUserId(int userId);
}

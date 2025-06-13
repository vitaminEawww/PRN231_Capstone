using System;

namespace Services.IServices;

public interface IDataSeederService
{
    Task SeedDefaultDataAsync();
    Task SeedAdminUserAsync();
    Task SeedSampleUsersAsync();
}

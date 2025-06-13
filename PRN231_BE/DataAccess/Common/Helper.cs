using System;

namespace DataAccess.Common;

public static class Helper
{
    /// <summary>
    /// hash password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// verify password
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}

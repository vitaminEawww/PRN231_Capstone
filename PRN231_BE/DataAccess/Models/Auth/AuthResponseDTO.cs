using System;

namespace DataAccess.Models.Auth;

public class AuthResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserInfoDTO User { get; set; } = null!;
}

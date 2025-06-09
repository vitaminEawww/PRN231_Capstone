using System.Net;

namespace DataAccess.Common;

public class ApiResponse
{
    public ApiResponse()
    {
        ErrorMessages = new List<string>();
    }

    public bool IsSuccess { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public object? Result { get; set; }
}
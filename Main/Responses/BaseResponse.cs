using Main.Enums;

namespace Main.Responses;

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; }
}

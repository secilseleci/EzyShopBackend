namespace Core.Utilities.Responses;

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = null!;
    public string? Detail { get; set; }
}

namespace API.Errors;

public class ApiErrorResponse(int statusCode, string message, string? details)
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }
}

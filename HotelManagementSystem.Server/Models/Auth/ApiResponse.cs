namespace HotelManagementSystem.Server.Models.Auth;

public class ApiResponse<T>(bool success, string? message = null, T? data = default)
{
    public bool Success { get; set; } = success;
    public string? Message { get; set; } = message;
    public T? Data { get; set; } = data;

    public static ApiResponse<T> SuccessResponse(T data, string? message = "Request successful") =>
        new(true, message, data);

    public static ApiResponse<T> FailureResponse(string message) =>
        new(false, message, default);
}
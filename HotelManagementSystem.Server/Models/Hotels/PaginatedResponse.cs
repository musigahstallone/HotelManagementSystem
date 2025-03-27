using HotelManagementSystem.Server.Models.Auth;

namespace HotelManagementSystem.Server.Models.Hotels;

public class PaginatedResponse<T>(IEnumerable<T> data, int page, int pageSize, int totalCount, string message = "Request successful") : ApiResponse<IEnumerable<T>>(true, message, data)
{
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

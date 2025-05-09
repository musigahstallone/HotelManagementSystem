namespace HotelManagementSystem.Server.Models.Hotels;

// Models for request bodies
public class BookingRequest
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
}

public class BookingDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid RoomId { get; init; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public decimal TotalAmount { get; init; }
    public bool IsPaid { get; init; }
}

public class BookingDetailsDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string RoomName { get; init; } = default!;
    public string? HotelName { get; init; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public decimal TotalAmount { get; init; }
    public bool IsPaid { get; init; }
}
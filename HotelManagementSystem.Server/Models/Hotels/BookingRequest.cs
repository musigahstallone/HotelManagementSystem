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

    public static BookingDetailsDto FromEntity(Booking b) => new()
    {
        Id = b.Id,
        UserName = b.User?.Name ?? "[unknown user]",
        RoomName = b.Room?.RoomNumber ?? "[unknown room]",
        HotelName = b.Room?.Hotel?.Name ?? "[unknown hotel]",
        CheckInDate = b.CheckInDate,
        CheckOutDate = b.CheckOutDate,
        TotalAmount = b.TotalAmount,
        IsPaid = b.IsPaid
    };
}

public class ExtendBookingRequest
{
    public DateOnly NewCheckOutDate { get; set; }
    public decimal AdditionalAmount { get; set; }
}

public class ChangeBookingDatesRequest
{
    public DateOnly NewCheckIn { get; set; }
    public DateOnly NewCheckOut { get; set; }
}

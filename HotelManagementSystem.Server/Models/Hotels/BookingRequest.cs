namespace HotelManagementSystem.Server.Models.Hotels;

// Models for request bodies
public class BookingRequest
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
}

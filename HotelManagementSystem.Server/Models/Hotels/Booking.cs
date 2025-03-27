using HotelManagementSystem.Server.Models.Auth;

namespace HotelManagementSystem.Server.Models.Hotels;

public class Booking
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public User? User { get; private set; } // Loaded from DB when needed
    public Guid RoomId { get; private set; }
    public Room? Room { get; private set; } // Loaded from DB when needed
    public DateOnly CheckInDate { get; private set; }
    public DateOnly CheckOutDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsPaid { get; private set; }
    public bool IsCancelled { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; private set; }

    private Booking() { }

    private Booking(Guid userId, Guid roomId, DateOnly checkIn, DateOnly checkOut, decimal totalAmount)
    {
        if (checkOut <= checkIn)
            throw new ArgumentException("Check-out date must be after check-in date.");

        UserId = userId;
        RoomId = roomId;
        CheckInDate = checkIn;
        CheckOutDate = checkOut;
        TotalAmount = totalAmount;
    }

    public static Booking CreateNew(Guid userId, Guid roomId, DateOnly checkIn, DateOnly checkOut, decimal totalAmount)
    {
        return new Booking(userId, roomId, checkIn, checkOut, totalAmount);
    }

    public void MarkAsPaid()
    {
        if (IsPaid)
            throw new InvalidOperationException("Booking is already paid.");

        IsPaid = true;
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Booking is already cancelled.");

        IsCancelled = true;
        CancelledAt = DateTime.UtcNow;
    }

    public void ExtendBooking(DateOnly newCheckOutDate, decimal additionalAmount)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot extend a cancelled booking.");

        if (newCheckOutDate <= CheckOutDate)
            throw new ArgumentException("New check-out date must be after the current check-out date.");

        CheckOutDate = newCheckOutDate;
        TotalAmount += additionalAmount;
    }

    public void ChangeDates(DateOnly newCheckIn, DateOnly newCheckOut)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot change dates for a cancelled booking.");

        if (newCheckOut <= newCheckIn)
            throw new ArgumentException("Check-out date must be after check-in date.");

        CheckInDate = newCheckIn;
        CheckOutDate = newCheckOut;
    }

    public int GetTotalNights() => CheckOutDate.DayNumber - CheckInDate.DayNumber;

    public bool IsUpcoming() => CheckInDate > DateOnly.FromDateTime(DateTime.UtcNow);

    public bool IsOngoing() =>
        DateOnly.FromDateTime(DateTime.UtcNow) >= CheckInDate &&
        DateOnly.FromDateTime(DateTime.UtcNow) < CheckOutDate;

    public bool IsCompleted() => DateOnly.FromDateTime(DateTime.UtcNow) >= CheckOutDate;

    public override string ToString()
    {
        return $"Booking {Id} - User: {UserId}, Room: {RoomId}, Check-In: {CheckInDate}, Check-Out: {CheckOutDate}, " +
               $"Total: {TotalAmount:C}, Paid: {IsPaid}, Cancelled: {IsCancelled}";
    }
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
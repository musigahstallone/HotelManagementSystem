namespace HotelManagementSystem.Server.Models.Hotels;

public class Payment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookingId { get; private set; }
    public Booking Booking { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateOnly PaymentDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    private Payment() { }

    private Payment(Booking booking, decimal amount)
    {
        Booking = booking;
        BookingId = booking.Id;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    public static Payment CreateNew(Booking booking, decimal amount)
    {
        return new Payment(booking, amount);
    }

    public void MarkAsCompleted() => Status = PaymentStatus.Completed;
    public void MarkAsFailed() => Status = PaymentStatus.Failed;
}
public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}
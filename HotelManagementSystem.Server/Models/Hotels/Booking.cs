using System;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Auth;
using HotelManagementSystem.Server.Models.Hotels;

namespace HotelManagementSystem.Server.Models.Hotels
{
    public class Booking
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }

        [JsonIgnore]
        public User? User { get; private set; }
        public Guid RoomId { get; private set; }

        [JsonIgnore]
        public Room? Room { get; private set; }
        public DateOnly CheckInDate { get; private set; }
        public DateOnly CheckOutDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsPaid { get; private set; }
        public bool IsCancelled { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? CancelledAt { get; private set; }

        private Booking() { }

        private Booking(Guid userId, Guid roomId, DateOnly checkIn, DateOnly checkOut, decimal pricePerNight)
        {
            if (checkIn >= checkOut)
                throw new ArgumentException("Check-in date must be before check-out date.");

            var nights = checkOut.DayNumber - checkIn.DayNumber;
            if (nights < 2)
                throw new ArgumentException("Booking must be for at least 2 nights.");

            UserId = userId;
            RoomId = roomId;
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            TotalAmount = nights * pricePerNight;
            IsCancelled = false;
        }

        public static async Task<Booking> CreateNewAsync(ApplicationDbContext context, BookingRequest request)
        {
            // validations
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.CheckInDate < today)
                throw new ArgumentException("Check-in date cannot be in the past.");
            if (request.CheckInDate >= request.CheckOutDate)
                throw new ArgumentException("Check-in date must be before check-out date.");

            // load related entities
            var user = await context.Users.FindAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var room = await context.Rooms.FindAsync(request.RoomId);
            if (room == null)
                throw new InvalidOperationException("Room not found.");

            // create booking
            return new Booking(user.Id, room.Id, request.CheckInDate, request.CheckOutDate, room.PricePerNight);
        }

        public void MarkAsPaid()
        {
            if (IsPaid)
                throw new InvalidOperationException("Booking is already marked as paid.");
            if (IsCancelled)
                throw new InvalidOperationException("Cannot mark a cancelled booking as paid.");
            IsPaid = true;
        }

        public void Cancel()
        {
            if (IsCancelled)
                throw new InvalidOperationException("Booking is already cancelled.");
            IsCancelled = true;
            CancelledAt = DateTime.UtcNow;
        }
        public void UnCancel()
        {
            if (!IsCancelled)
                throw new InvalidOperationException("Booking is not cancelled.");

            IsCancelled = false;
            CancelledAt = null;
        }

        public void ExtendBooking(DateOnly newCheckOutDate, decimal pricePerNight, decimal extensionFee)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot extend a cancelled booking.");
            if (newCheckOutDate <= CheckOutDate)
                throw new ArgumentException("New check-out date must be after the current check-out date.");
            if (newCheckOutDate <= CheckInDate)
                throw new ArgumentException("New check-out date must be after the check-in date.");

            // Calculate new total nights
            var nights = newCheckOutDate.DayNumber - CheckInDate.DayNumber;
            if (nights < 2)
                throw new ArgumentException("Booking must be for at least 2 nights total.");

            CheckOutDate = newCheckOutDate;
            TotalAmount = (nights * pricePerNight) + extensionFee;
        }

        public void ChangeDates(DateOnly newCheckIn, DateOnly newCheckOut)
        {
            if (newCheckIn >= newCheckOut)
                throw new ArgumentException("Check-in date must be before check-out date.");
            var nights = newCheckOut.DayNumber - newCheckIn.DayNumber;
            if (nights < 2)
                throw new ArgumentException("Booking must be for at least 2 nights.");

            CheckInDate = newCheckIn;
            CheckOutDate = newCheckOut;
            TotalAmount = nights * Room.PricePerNight;
            // Note: TotalAmount recalculation requires pricePerNight context
        }

        public int GetTotalNights() => CheckOutDate.DayNumber - CheckInDate.DayNumber;
        public bool IsUpcoming() => CheckInDate > DateOnly.FromDateTime(DateTime.UtcNow);
        public bool IsOngoing() => DateOnly.FromDateTime(DateTime.UtcNow) >= CheckInDate && DateOnly.FromDateTime(DateTime.UtcNow) < CheckOutDate;
        public bool IsCompleted() => DateOnly.FromDateTime(DateTime.UtcNow) >= CheckOutDate;

        public override string ToString()
        {
            return $"Booking {Id} - User: {UserId}, Room: {RoomId}, Check-In: {CheckInDate:yyyy-MM-dd}, Check-Out: {CheckOutDate:yyyy-MM-dd}, Total: {TotalAmount:C}, Paid: {IsPaid}, Cancelled: {IsCancelled}";
        }
    }
}
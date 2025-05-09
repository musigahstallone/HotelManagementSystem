using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;
using Microsoft.AspNetCore.Http.HttpResults;
using Humanizer;

namespace HotelManagementSystem.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    public IQueryable<T> GetEntityQuery<T>() where T: class => _context.Set<T>().AsQueryable();

    public static async Task<PaginatedResponse<T>> GetPaginatedData<T>(int page, int pageSize, IQueryable<T> query)
    {
        if (page < 1 || pageSize < 1)
            throw new ArgumentException("Page and pageSize must be greater than zero.");

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<T>(items, page, pageSize, totalCount);
    }



    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<BookingDetailsDto>>> GetBookings(
   [FromQuery] int page = 1,
   [FromQuery] int pageSize = 10)
    {
        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .OrderBy(b => b.Id)
            .Select(b => new BookingDetailsDto
            {
                Id = b.Id,
                UserName = b.User.Name,
                RoomName = b.Room.RoomNumber,
                HotelName = b.Room.Hotel.Name,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount,
                IsPaid = b.IsPaid
            });

        return Ok(await GetPaginatedData(page, pageSize, query));
    }


    [HttpGet("all-bookings")]
    public async Task<ActionResult<PaginatedResponse<BookingDetailsDto>>> GetPagedBookings(
      [FromQuery] int pageSize,
      [FromQuery] int pageNumber)
    {
        var query = GetEntityQuery<Booking>()
            .Include(b => b.User)
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .OrderBy(b => b.Id)
            .Select(b => new BookingDetailsDto
            {
                Id = b.Id,
                UserName = b.User.Name,
                RoomName = b.Room.RoomNumber,
                HotelName = b.Room.Hotel.Name,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount,
                IsPaid = b.IsPaid
            });

        return Ok(await GetPaginatedData(pageNumber, pageSize, query));
    }


    /*[HttpGet("summary")]
    public async Task<ActionResult<string>> GetBookingSummary(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound("Booking Not Found");
        }

        return Ok(booking.ToString());
    }*/

    [HttpGet("summary")]
    public async Task<ActionResult<string>> GetBookingSummary(Guid id)
    {
        // eagerly load User, Room and Hotel
        var booking = await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room!)
                .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound("Booking not found.");

        // guard against missing navigation (shouldn’t happen if your FK’s are intact)
        var userName = booking.User?.Name ?? "[unknown user]";
        var roomNum = booking.Room?.RoomNumber ?? "[unknown room]";
        var hotelName = booking.Room?.Hotel?.Name ?? "[unknown hotel]";

        var paidText = booking.IsPaid ? "Paid" : "Not paid";
        var cancelledText = booking.IsCancelled ? "Cancelled" : "Active";

        var summary =
            $"Booking:   {userName} in room {roomNum} at {hotelName}.\n" +
            $"Check‑in:   {booking.CheckInDate:yyyy‑MM‑dd}\n" +
            $"Check‑out:   {booking.CheckOutDate:yyyy‑MM‑dd}\n" +
            $"Nights:   {booking.GetTotalNights()}\n" +
            $"Amount:   {booking.TotalAmount:C}\n" +
            $"Status:   {paidText}, {cancelledText}";

        return Ok(summary);
    }

    // GET: api/Bookings/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBookings(Guid userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)
            .Include(b => b.Room.Hotel)
            .ToListAsync();

        if (bookings.Count == 0)
            return NotFound("No bookings found for the user.");

        return Ok(bookings);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();
        return booking;
    }

    [HttpPost("create-booking")]
    public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingRequest request)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Validation: Check-in must be today or later
        if (request.CheckInDate < currentDate)
            return BadRequest("Check-in date cannot be in the past.");

        // Validation: Check-in must be before check-out
        if (request.CheckInDate >= request.CheckOutDate)
            return BadRequest("Check-in date must be before check-out date.");

        // Validation: Minimum stay should be at least 2 days
        if ((request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber) < 2)
            return BadRequest("Check-out date must be at least 2 days after check-in date.");

        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return NotFound("User not found.");

        var room = await _context.Rooms.FindAsync(request.RoomId);
        if (room == null)
            return NotFound("Room not found.");

        var booking = Booking.CreateNew(request.UserId, request.RoomId, request.CheckInDate, request.CheckOutDate, room.PricePerNight);

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
     
        var dto = new BookingDetailsDto
        {
            Id = booking.Id,
            UserName = user.Name,
            RoomName = room.RoomNumber,
            HotelName = room.Hotel?.Name,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalAmount = booking.TotalAmount,
            IsPaid = booking.IsPaid
        };

        return CreatedAtAction(nameof(GetBooking), new { id = dto.Id }, dto);

    }

    [HttpPut("{id}/mark-paid")]
    public async Task<IActionResult> MarkAsPaid(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        try
        {
            booking.MarkAsPaid();
            await _context.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        try
        {
            booking.Cancel();
            await _context.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [HttpPut("{id}/extend")]
    public async Task<IActionResult> ExtendBooking(Guid id, [FromBody] ExtendBookingRequest request)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        try
        {
            booking.ExtendBooking(request.NewCheckOutDate, request.AdditionalAmount);
            await _context.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [HttpPut("{id}/change-dates")]
    public async Task<IActionResult> ChangeDates(Guid id, [FromBody] ChangeBookingDatesRequest request)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        try
        {
            booking.ChangeDates(request.NewCheckIn, request.NewCheckOut);
            await _context.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

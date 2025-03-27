using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;
using Microsoft.AspNetCore.Http.HttpResults;

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
    public async Task<ActionResult<PaginatedResponse<Booking>>> GetBookings(
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 10)
    {
        var totalCount = await _context.Bookings.CountAsync();
        var items = await _context.Bookings
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PaginatedResponse<Booking>(items, page, pageSize, totalCount));
    }

    [HttpGet("all-bookings")]
    public async Task<ActionResult<Booking>> GetPagedBookings([FromQuery]int pageSize, [FromQuery]int pageNumber)
    {
        //var bookings = await _context.Bookings
        //    .Skip((pageNumber - 1) * pageSize)
        //    .Take(pageSize)
        //    .ToListAsync();

        var bookings = await GetPaginatedData(pageNumber, pageSize, GetEntityQuery<Booking>().OrderBy(t => t.Id));

        return Ok(bookings);

    }

    [HttpGet("summary")]
    public async Task<ActionResult<string>> GetBookingSummary(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound("Booking Not Found");
        }

        return Ok(booking.ToString());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();
        return booking;
    }

    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingRequest request)
    {
        if (request.CheckOutDate <= request.CheckInDate)
            return BadRequest("Check-out date must be after check-in date.");

        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists) return NotFound("User not found.");

        var roomExists = await _context.Rooms.AnyAsync(r => r.Id == request.RoomId);
        if (!roomExists) return NotFound("Room not found.");

        var booking = Booking.CreateNew(request.UserId, request.RoomId, request.CheckInDate, request.CheckOutDate, request.TotalAmount);

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
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
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(ApplicationDbContext context, ILogger<BookingsController> logger) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<BookingsController> _logger = logger;

    private IQueryable<T> Query<T>() where T : class => _context.Set<T>().AsNoTracking();

    private static async Task<PaginatedResponse<T>> PaginateAsync<T>(IQueryable<T> query, int page, int pageSize)
    {
        if (page < 1 || pageSize < 1)
            throw new ArgumentException("Page and pageSize must be greater than zero.");

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResponse<T>(items, page, pageSize, total);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<BookingDetailsDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Starting GetAll bookings request (Page: {Page}, PageSize: {PageSize})", page, pageSize);

        var bookings = Query<Booking>()
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r.Hotel)
            .OrderBy(b => b.CreatedAt)
            .Select(b => BookingDetailsDto.FromEntity(b));

        var result = await PaginateAsync(bookings, page, pageSize);

        _logger.LogInformation("Finished GetAll bookings request. Retrieved {Count} items.", result.Data?.Count());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingDetailsDto>> GetById(Guid id)
    {
        _logger.LogInformation("Starting GetById for booking {BookingId}", id);

        var booking = await Query<Booking>()
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            _logger.LogWarning("Booking with ID {BookingId} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Finished GetById for booking {BookingId}", id);
        return Ok(BookingDetailsDto.FromEntity(booking));
    }

    [HttpGet("{id:guid}/summary")]
    public async Task<ActionResult<string>> Summary(Guid id)
    {
        _logger.LogInformation("Starting Summary for booking {BookingId}", id);

        var booking = await Query<Booking>()
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            _logger.LogWarning("Booking summary request failed. Booking {BookingId} not found.", id);
            return NotFound("Booking not found.");
        }
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

        //return Ok(summary);

        _logger.LogInformation("Finished Summary for booking {BookingId}", id);
        return Ok(summary);
        //return Ok(booking.ToString());
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<BookingDetailsDto>>> GetByUser(Guid userId)
    {
        _logger.LogInformation("Starting GetByUser for user {UserId}", userId);

        var bookings = await Query<Booking>()
            .Where(b => b.UserId == userId)
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r.Hotel)
            .OrderByDescending(b => b.CheckInDate)
            .ToListAsync();

        if (!bookings.Any())
        {
            _logger.LogInformation("No bookings found for user {UserId}.", userId);
            return NotFound("No bookings found for this user.");
        }

        _logger.LogInformation("Finished GetByUser for user {UserId}. Found {Count} bookings.", userId, bookings.Count);
        return Ok(bookings.Select(BookingDetailsDto.FromEntity));
    }

    [HttpPost]
    public async Task<ActionResult<BookingDetailsDto>> Create([FromBody] BookingRequest request)
    {
        _logger.LogInformation("Starting Create booking for user {UserId}", request.UserId);

        try
        {
            var booking = await Booking.CreateNewAsync(_context, request);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created booking {BookingId} for user {UserId}", booking.Id, booking.UserId);
            var dto = BookingDetailsDto.FromEntity(booking);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            _logger.LogWarning(ex, "Failed to create booking.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/pay")]
    public async Task<IActionResult> MarkPaid(Guid id)
    {
        _logger.LogInformation("Starting MarkPaid for booking {BookingId}", id);

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for MarkPaid.", id);
            return NotFound();
        }

        try
        {
            booking.MarkAsPaid();
            await _context.SaveChangesAsync();
            _logger.LogInformation("Finished MarkPaid for booking {BookingId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking booking {BookingId} as paid.", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        _logger.LogInformation("Starting cancel for booking {BookingId}", id);

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for cancel.", id);
            return NotFound();
        }

        try
        {
            booking.Cancel();
            await _context.SaveChangesAsync();
            _logger.LogInformation("Finished cancel for booking {BookingId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/uncancel")]
    public async Task<IActionResult> UnCancel(Guid id)
    {
        _logger.LogInformation("Starting uncancel for booking {BookingId}", id);

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for uncancel.", id);
            return NotFound();
        }

        try
        {
            booking.UnCancel();
            await _context.SaveChangesAsync();
            _logger.LogInformation("Finished uncancel for booking {BookingId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uncancelling booking {BookingId}", id);
            return BadRequest(ex.Message);
        }
    }


    [HttpPut("{id:guid}/extend")]
    public async Task<IActionResult> Extend(Guid id, [FromBody] ExtendBookingRequest request)
    {
        _logger.LogInformation("Starting Extend for booking {BookingId}", id);

        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for Extend.", id);
            return NotFound();
        }

        try
        {
            booking.ExtendBooking(request.NewCheckOutDate, booking.Room.PricePerNight, request.AdditionalAmount);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Finished Extend for booking {BookingId}", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid extend request for booking {BookingId}", id);
            return BadRequest(ex.Message);
        }
    }


    [HttpPut("{id:guid}/change-dates")]
    public async Task<IActionResult> ChangeDates(Guid id, [FromBody] ChangeBookingDatesRequest request)
    {
        _logger.LogInformation("Starting ChangeDates for booking {BookingId}", id);

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for ChangeDates.", id);
            return NotFound();
        }

        try
        {
            booking.ChangeDates(request.NewCheckIn, request.NewCheckOut);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Finished ChangeDates for booking {BookingId}", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid change dates request for booking {BookingId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Starting Delete for booking {BookingId}", id);

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for Delete.", id);
            return NotFound();
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Finished Delete for booking {BookingId}", id);

        return NoContent();
    }
}
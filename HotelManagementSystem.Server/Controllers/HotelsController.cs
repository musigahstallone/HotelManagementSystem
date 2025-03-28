using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;

namespace HotelManagementSystem.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    public IQueryable<T> GetEntityQuery<T>() where T : class => _context.Set<T>().AsQueryable();

    public static async Task<PaginatedResponse<T>> GetPaginatedData<T>(
      int page, int pageSize, IQueryable<T> query) where T : class
    {
        if (page < 1 || pageSize < 1)
            throw new ArgumentException("Page and PageSize must be greater than zero.");

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<T>(items, totalCount, pageSize, page);
    }

    /*public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels() => await _context.Hotels.ToListAsync(); //[HttpGet("all-hotels")]*/

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<HotelDto>>> GetAllHotels(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 5)
    {
        var query = GetEntityQuery<Hotel>()
        .OrderBy(h => h.Id)
        .Select(h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            Location = h.Location,
            ContactEmail = h.ContactEmail,
            Website = h.Website
        });

        var hotels = await GetPaginatedData(pageNumber,pageSize, query);

        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return NotFound();

        return hotel;
    }
    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel([FromBody] HotelUpdate hotelDto)
    {
        if (hotelDto == null)
            return BadRequest("Invalid hotel data.");

        if (string.IsNullOrWhiteSpace(hotelDto.Name) || string.IsNullOrWhiteSpace(hotelDto.Location))
            return BadRequest("Hotel name and location are required.");

        var newHotel = Hotel.CreateNew(
            hotelDto.Name,
            hotelDto.Location,
            hotelDto.Description ?? string.Empty,
            hotelDto.ContactEmail ?? string.Empty,
            hotelDto.ContactPhone ?? string.Empty,
            hotelDto.Website ?? string.Empty
        );

        _context.Hotels.Add(newHotel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHotel), new { id = newHotel.Id }, newHotel);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] HotelUpdate hotel)
    {
        if (id != hotel.Id)
            return BadRequest("Mismatched hotel ID.");

        var existingHotel = await _context.Hotels.FindAsync(id);
        if (existingHotel == null)
            return NotFound();

        existingHotel.UpdateDetails(
            hotel.Name, 
            hotel.Location,
            hotel.Description,
            hotel.ContactEmail ?? string.Empty,
            hotel.ContactPhone ?? string.Empty,
            hotel.Website
            );

        _context.Entry(existingHotel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HotelExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateHotel(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return NotFound();

        hotel.Deactivate();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ReactivateHotel(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return NotFound();

        hotel.Reactivate();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return NotFound();

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool HotelExists(Guid id)
    {
        return _context.Hotels.Any(e => e.Id == id);
    }
}

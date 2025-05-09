using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
    {
        var rooms = await _context.Rooms.ToListAsync();
        return Ok(rooms.Select(RoomDTO.FromEntity));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDTO>> GetRoom(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        return room is null ? NotFound() : Ok(RoomDTO.FromEntity(room));
    }

    [HttpPost]
    public async Task<ActionResult<RoomDTO>> PostRoom(RoomCreateRequest request)
    {
        var hotel = await _context.Hotels.FindAsync(request.HotelId);
        if (hotel is null)
            return BadRequest("Invalid hotel.");

        var room = new Room(request.RoomNumber, request.PricePerNight, request.Type, request.Capacity, hotel);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, RoomDTO.FromEntity(room));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRoom(Guid id, RoomUpdateRequest request)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room is null) return NotFound();

        room.UpdatePrice(request.PricePerNight);
        room.ChangeAvailability(request.IsAvailable);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room is null) return NotFound();

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
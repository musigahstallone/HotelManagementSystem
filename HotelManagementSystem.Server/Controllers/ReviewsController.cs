using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;

namespace HotelManagementSystem.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    // GET: api/Reviews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> GetReview()
    {
        return await _context.Review.ToListAsync();
    }

    // GET: api/Reviews/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(Guid id)
    {
        var review = await _context.Review.FindAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        return review;
    }

    // PUT: api/Reviews/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutReview(Guid id, Review review)
    {
        if (id != review.Id)
        {
            return BadRequest();
        }

        _context.Entry(review).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReviewExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Reviews
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Review>> PostReview(Review review)
    {
        _context.Review.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetReview", new { id = review.Id }, review);
    }

    // DELETE: api/Reviews/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var review = await _context.Review.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        _context.Review.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ReviewExists(Guid id)
    {
        return _context.Review.Any(e => e.Id == id);
    }
}

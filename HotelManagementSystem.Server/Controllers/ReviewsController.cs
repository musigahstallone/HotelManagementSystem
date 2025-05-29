using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Auth;
using HotelManagementSystem.Server.Models.Hotels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("all-reviews")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
    {
        _logger.LogInformation("Fetching all reviews.");

        var reviews = await _context.Review
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Hotel)
            .ThenInclude(h => h.Rooms)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                UserName = r.User.Name, 
                UserEmail = r.User.Email,
                HotelName = r.Hotel.Name,
                HotelLocation = r.Hotel.Location,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate
            })
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} reviews.", reviews.Count);

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReview(Guid id)
    {
        _logger.LogInformation("Fetching review with ID: {Id}", id);

        var r = await _context.Review
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Hotel).ThenInclude(h => h.Rooms)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (r == null)
        {
            _logger.LogWarning("Review with ID {Id} not found.", id);
            return NotFound();
        }

        var dto = new ReviewDto
        {
            Id = r.Id,
            UserName = r.User.Name, 
            UserEmail = r.User.Email ,
            HotelName = r.Hotel.Name,
            HotelLocation = r.Hotel.Location,
            Rating = r.Rating,
            Comment = r.Comment,
            ReviewDate = r.ReviewDate
        };

        _logger.LogInformation("Returning review for ID: {Id}", id);
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutReview(Guid id, UpdateReviewDto input)
    {
        _logger.LogInformation("Updating review with ID: {Id}", id);

        if (id != input.Id)
        {
            _logger.LogWarning("Review ID mismatch. URL ID: {Id}, Body ID: {BodyId}", id, input.Id);
            return BadRequest("Mismatched review ID.");
        }

        var review = await _context.Review.FindAsync(id);
        if (review == null)
        {
            _logger.LogWarning("Review with ID {Id} not found for update.", id);
            return NotFound();
        }

        try
        {
            review.UpdateReview(input.Rating, input.Comment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Review with ID {Id} updated successfully.", id);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!ReviewExists(id))
            {
                _logger.LogWarning("Review with ID {Id} does not exist anymore during update.", id);
                return NotFound();
            }

            _logger.LogError(ex, "Concurrency error while updating review ID {Id}", id);
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> PostReview(CreateReviewDto input)
    {
        _logger.LogInformation("Creating a new review.");

        var user = await _context.Users.FindAsync(input.UserId);
        if (user == null)
        {
            _logger.LogWarning("Invalid user ID: {UserId}", input.UserId);
            return BadRequest("Invalid user.");
        }

        var hotel = await _context.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == input.HotelId);
        if (hotel == null)
        {
            _logger.LogWarning("Invalid hotel ID: {HotelId}", input.HotelId);
            return BadRequest("Invalid hotel.");
        }

        try
        {
            var review = Review.Create(input.UserId, input.HotelId, input.Rating, input.Comment);
            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            var dto = new ReviewDto
            {
                Id = review.Id,
                UserName = user.Name, 
                UserEmail = user.Email,
                HotelName = hotel.Name,
                HotelLocation = hotel.Location,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate
            };

            _logger.LogInformation("Review created with ID: {Id}", dto.Id);

            return CreatedAtAction(nameof(GetReview), new { id = dto.Id }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating review.");
            return StatusCode(500, "An error occurred while creating the review.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        _logger.LogInformation("Deleting review with ID: {Id}", id);

        var review = await _context.Review.FindAsync(id);
        if (review == null)
        {
            _logger.LogWarning("Review with ID {Id} not found for deletion.", id);
            return NotFound();
        }

        _context.Review.Remove(review);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Review with ID {Id} deleted successfully.", id);

        return NoContent();
    }

    private bool ReviewExists(Guid id)
    {
        return _context.Review.Any(e => e.Id == id);
    }
}

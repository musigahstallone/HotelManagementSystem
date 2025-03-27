using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;

namespace HotelManagementSystem.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    // GET: api/Payments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPayment()
    {
        return await _context.Payment.ToListAsync();
    }

    // GET: api/Payments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetPayment(Guid id)
    {
        var payment = await _context.Payment.FindAsync(id);

        if (payment == null)
        {
            return NotFound();
        }

        return payment;
    }

    // PUT: api/Payments/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPayment(Guid id, Payment payment)
    {
        if (id != payment.Id)
        {
            return BadRequest();
        }

        _context.Entry(payment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PaymentExists(id))
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

    // POST: api/Payments
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Payment>> PostPayment(Payment payment)
    {
        _context.Payment.Add(payment);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPayment", new { id = payment.Id }, payment);
    }

    // DELETE: api/Payments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(Guid id)
    {
        var payment = await _context.Payment.FindAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        _context.Payment.Remove(payment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PaymentExists(Guid id)
    {
        return _context.Payment.Any(e => e.Id == id);
    }
}

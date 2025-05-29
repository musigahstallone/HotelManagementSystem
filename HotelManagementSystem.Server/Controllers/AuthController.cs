using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Composition;

namespace HotelManagementSystem.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ApplicationDbContext context, TokenService tokenService) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly TokenService _tokenService = tokenService;

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already exists");
        
        var user = Models.Auth.User.Create(dto.Name, dto.Email, dto.Password, dto.Role);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { User = user.ToDto(), token = _tokenService.GenerateToken(user) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !user.VerifyPassword(dto.Password))
            return Unauthorized("Invalid credentials");

        return Ok(new { User = user.ToDto(), token = _tokenService.GenerateToken(user) });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return Ok(new { message = "User doesn't exist!. Try a different email" });

        var otp = new Random().Next(100000, 999999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(10);
        user.SetOtp(otp, expiry);

        await _context.SaveChangesAsync();

        // TODO: Send OTP via email
        return Ok(new
        {
            OTP = otp,
            message = "OTP sent",
            OtpExpiryDate = user.OtpExpiryDate,
            OtpExpiryTime = user.OtpExpiryTime
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return BadRequest(new { message = "User doesn't exist!. Try a different email" });

        if (!user.VerifyOtp(dto.Otp))
            return BadRequest("Invalid or expired OTP");

        user.UpdatePassword(dto.NewPassword);
        await _context.SaveChangesAsync();

        return Ok("Password updated");
    }
}

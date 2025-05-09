namespace HotelManagementSystem.Server.Models.Auth;
public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public string? OtpCode { get; private set; }
    public DateOnly? OtpExpiryDate { get; private set; }
    public TimeOnly? OtpExpiryTime { get; private set; }

    private User() { }

    private User(string name, string email, string password, UserRole role)
    {
        Name = name;
        Email = email;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        Role = role;
    }

    public static User Create(string name, string email, string password, UserRole role) =>
        new(name, email, password, role);

    public User UpdateRole(UserRole newRole)
    {
        Role = newRole;
        return this;
    }

    public bool VerifyPassword(string password) =>
        BCrypt.Net.BCrypt.Verify(password, PasswordHash);

    public void SetOtp(string code, DateTime expiry)
    {
        OtpCode = code;
        // split the incoming DateTime into date + time
        OtpExpiryDate = DateOnly.FromDateTime(expiry);
        OtpExpiryTime = TimeOnly.FromDateTime(expiry);
    }

    public bool VerifyOtp(string code)
    {
        if (OtpCode != code || OtpExpiryDate == null || OtpExpiryTime == null)
            return false;

        // rebuild a DateTime to compare
        var expiry = OtpExpiryDate.Value.ToDateTime(OtpExpiryTime.Value);
        return expiry > DateTime.UtcNow;
    }

    // this method is called when the user requests a password reset
    // this method is called when the user successfully resets their password
    public void UpdatePassword(string newPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        OtpCode = null;
        OtpExpiryDate = null;
        OtpExpiryTime = null;
    }
}


public enum UserRole
{
    Customer,
    Admin,
    Manager,
    Staff,
    Supervisor,
    Receptionist,
    Accountant,
    // …any others you need
}

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role
);

/*string? OtpCode,DateOnly? OtpExpiryDate,TimeOnly? OtpExpiryTime*/

public record SignupDto(string Name, string Email, string Password, UserRole Role);
public record LoginDto(string Email, string Password);
public record ForgotPasswordDto(string Email);
public record ResetPasswordDto(string Email, string Otp, string NewPassword, DateOnly ExpiryDate, TimeOnly ExpiryTime);
public record SetOtpDto(string Email, string Otp, DateOnly ExpiryDate, TimeOnly ExpiryTime);
public record UpdateUserRoleDto(UserRole Role);

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.Role.ToString() // 👈 Enum to string
        );
    }
}

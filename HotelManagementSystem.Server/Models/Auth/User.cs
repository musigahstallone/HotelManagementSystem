namespace HotelManagementSystem.Server.Models.Auth;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    private User() { }

    private User(string name, string email, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Name and Email cannot be empty.");

        Name = name;
        Email = email;
        Role = role;
    }

    public static User CreateNew(string name, string email, UserRole role)
    {
        return new User(name, email, role);
    }
}

public enum UserRole
{
    Customer,
    Admin
}

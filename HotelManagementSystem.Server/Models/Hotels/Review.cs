using HotelManagementSystem.Server.Models.Auth;

namespace HotelManagementSystem.Server.Models.Hotels;

public class Review
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateOnly ReviewDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    private Review() { }

    private Review(User user, Hotel hotel, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        User = user;
        UserId = user.Id;
        Hotel = hotel;
        HotelId = hotel.Id;
        Rating = rating;
        Comment = comment;
    }

    public static Review CreateNew(User user, Hotel hotel, int rating, string comment)
    {
        return new Review(user, hotel, rating, comment);
    }
}

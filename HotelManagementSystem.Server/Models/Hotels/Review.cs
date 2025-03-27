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
    public string Comment { get; private set; } = string.Empty;
    public DateTime ReviewDate { get; private set; } = DateTime.UtcNow;

    private Review() { }

    private Review(User user, Hotel hotel, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment cannot be empty.");

        User = user;
        UserId = user.Id;
        Hotel = hotel;
        HotelId = hotel.Id;
        Rating = rating;
        Comment = comment;
        ReviewDate = DateTime.UtcNow;
    }

    public static Review CreateNew(User user, Hotel hotel, int rating, string comment)
    {
        return new Review(user, hotel, rating, comment);
    }

    public void UpdateReview(int newRating, string newComment)
    {
        if (newRating < 1 || newRating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(newComment))
            throw new ArgumentException("Comment cannot be empty.");

        Rating = newRating;
        Comment = newComment;
        ReviewDate = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{User.Name} rated {Hotel.Name} {Rating}★ on {ReviewDate:yyyy-MM-dd}: \"{Comment}\"";
    }
}

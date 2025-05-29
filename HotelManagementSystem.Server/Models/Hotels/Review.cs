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

    public static Review Create(Guid userId, Guid hotelId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment cannot be empty.");

        return new Review
        {
            UserId = userId,
            HotelId = hotelId,
            Rating = rating,
            Comment = comment,
            ReviewDate = DateTime.UtcNow
        };
    }

    public void UpdateReview(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment cannot be empty.");

        Rating = rating;
        Comment = comment;
        ReviewDate = DateTime.UtcNow;
    }

    public override string ToString()
        => $"{User.Name} rated {Hotel.Name} {Rating}★ on {ReviewDate:yyyy-MM-dd}: \"{Comment}\"";
}

public class ReviewDto
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string HotelName { get; set; } = null!;
    public string HotelLocation { get; set; } = null!;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewDate { get; set; }
}

public class CreateReviewDto
{
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class UpdateReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}


namespace HotelManagementSystem.Server.Models.Hotels;

public class Hotel
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public List<Room> Rooms { get; private set; } = [];
    public int StarRating { get; private set; } = 3; // Default to 3 stars
    public string ContactEmail { get; private set; } = string.Empty;
    public string ContactPhone { get; private set; } = string.Empty;
    public string Website { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public List<string> Amenities { get; private set; } = [];
    public List<string> Policies { get; private set; } = [];

    private Hotel() { }

    private Hotel(string name, string location, string description, int starRating, string contactEmail, string contactPhone, string website)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Hotel name and location cannot be empty.");

        if (starRating is < 1 or > 5)
            throw new ArgumentException("Star rating must be between 1 and 5.");

        Name = name;
        Location = location;
        Description = description;
        StarRating = starRating;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        Website = website;
    }

    public static Hotel CreateNew(string name, string location, string description, string contactEmail, string contactPhone, string website, int starRating = 3)
    {
        return new Hotel(name, location, description, starRating, contactEmail, contactPhone, website);
    }

    public void UpdateDetails(string name, string location,string contactEmail, string contactPhone, string description, string website)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Hotel name and location cannot be empty.");

        Name = name;
        Location = location;
        Description = description;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        Website = website;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRoom(Room room)
    {
        if (room == null)
            throw new ArgumentNullException(nameof(room));

        Rooms.Add(room);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveRoom(Room room)
    {
        if (room == null)
            throw new ArgumentNullException(nameof(room));

        Rooms.Remove(room);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAmenity(string amenity)
    {
        if (string.IsNullOrWhiteSpace(amenity))
            throw new ArgumentException("Amenity cannot be empty.");

        Amenities.Add(amenity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAmenity(string amenity)
    {
        Amenities.Remove(amenity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPolicy(string policy)
    {
        if (string.IsNullOrWhiteSpace(policy))
            throw new ArgumentException("Policy cannot be empty.");

        Policies.Add(policy);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePolicy(string policy)
    {
        Policies.Remove(policy);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStarRating(int newRating)
    {
        if (newRating is < 1 or > 5)
            throw new ArgumentException("Star rating must be between 1 and 5.");
        StarRating = newRating;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContactDetails(string email, string phone)
    {
        ContactEmail = email;
        ContactPhone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStarRating()
    {
        StarRating++;
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{Name} ({StarRating}★) - {Location}. {Description}. Contact: {ContactEmail}, {ContactPhone}. Website: {Website}.";
    }
}

namespace HotelManagementSystem.Server.Models.Hotels;

public class Room
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string RoomNumber { get; private set; } = string.Empty;
    public decimal PricePerNight { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public int Capacity { get; private set; }
    public List<Booking> Bookings { get; private set; } = new();

    public RoomType Type { get; private set; }

    private Room() { }

    private Room(string roomNumber, decimal pricePerNight, RoomType type, Hotel hotel)
    {
        if (pricePerNight <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        RoomNumber = roomNumber;
        PricePerNight = pricePerNight;
        Type = type;
        Hotel = hotel;
        HotelId = hotel.Id;
    }

    public static Room CreateNew(string roomNumber, decimal pricePerNight, RoomType type, Hotel hotel)
    {
        return new Room(roomNumber, pricePerNight, type, hotel);
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("New price must be greater than zero.");
        PricePerNight = newPrice;
    }

    public void MarkAsUnavailable() => IsAvailable = false;
    public void MarkAsAvailable() => IsAvailable = true;
}


public enum RoomType
{
    Single,
    Double,
    Suite,
    Deluxe
}

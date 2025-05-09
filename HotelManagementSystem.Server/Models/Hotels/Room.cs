namespace HotelManagementSystem.Server.Models.Hotels;

public class Room
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string RoomNumber { get; private set; }
    public decimal PricePerNight { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public int Capacity { get; private set; }
    public RoomType Type { get; private set; }

    private readonly List<Booking> _bookings = new();
    public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

    private Room() { }

    public Room(string roomNumber, decimal pricePerNight, RoomType type, int capacity, Hotel hotel)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("Room number is required.");
        if (pricePerNight <= 0)
            throw new ArgumentException("Price must be greater than zero.");
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.");

        RoomNumber = roomNumber;
        PricePerNight = pricePerNight;
        Type = type;
        Capacity = capacity;
        Hotel = hotel;
        HotelId = hotel.Id;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("New price must be greater than zero.");
        PricePerNight = newPrice;
    }

    public void ChangeAvailability(bool available) => IsAvailable = available;
}

public enum RoomType
{
    Single,
    Double,
    Suite,
    Deluxe
}

public class RoomDTO
{
    public Guid Id { get; init; }
    public string RoomNumber { get; init; } = string.Empty;
    public decimal PricePerNight { get; init; }
    public bool IsAvailable { get; init; }
    public int Capacity { get; init; }
    public RoomType Type { get; init; }

    public static RoomDTO FromEntity(Room room) => new()
    {
        Id = room.Id,
        RoomNumber = room.RoomNumber,
        PricePerNight = room.PricePerNight,
        IsAvailable = room.IsAvailable,
        Capacity = room.Capacity,
        Type = room.Type
    };
}

public class RoomCreateRequest
{
    public string RoomNumber { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public RoomType Type { get; set; }
    public Guid HotelId { get; set; }
}

public class RoomUpdateRequest
{
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
}

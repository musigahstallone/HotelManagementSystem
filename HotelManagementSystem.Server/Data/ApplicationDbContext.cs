using HotelManagementSystem.Server.Models.Auth;
using HotelManagementSystem.Server.Models.Hotels;
using HotelManagementSystem.Server.Models.Todo;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // === TODO Entity Configuration ===
        modelBuilder.Entity<Todo>()
            .HasIndex(t => t.Note);

        modelBuilder.Entity<Todo>()
            .Property(t => t.CreatedDate)
            .HasColumnType("date");

        modelBuilder.Entity<Todo>()
            .Property(t => t.CreatedTime)
            .HasColumnType("time");

        modelBuilder.Entity<Todo>()
            .Property(t => t.CompletedDate)
            .HasColumnType("date");

        modelBuilder.Entity<Todo>()
            .Property(t => t.CompletedTime)
            .HasColumnType("time");

        // === HOTEL Entity Configuration ===
        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.Name)
            .IsUnique(); // Hotel names should be unique

        modelBuilder.Entity<Hotel>()
            .Property(h => h.Name)
            .IsRequired();

        modelBuilder.Entity<Hotel>()
            .Property(h => h.Location)
            .IsRequired();

        modelBuilder.Entity<Hotel>()
            .HasMany(h => h.Rooms)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        // === ROOM Entity Configuration ===
        modelBuilder.Entity<Room>()
            .Property(r => r.PricePerNight)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HotelId);

        // === USER Entity Configuration ===
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .IsRequired();

        // === BOOKING Entity Configuration ===
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckInDate)
            .HasColumnType("date");

        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckOutDate)
            .HasColumnType("date");
    }

public DbSet<HotelManagementSystem.Server.Models.Hotels.Payment> Payment { get; set; } = default!;

public DbSet<HotelManagementSystem.Server.Models.Hotels.Review> Review { get; set; } = default!;
}

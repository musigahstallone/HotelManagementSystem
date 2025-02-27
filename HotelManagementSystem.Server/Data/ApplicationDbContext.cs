using HotelManagementSystem.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}

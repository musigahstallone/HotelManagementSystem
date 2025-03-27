using HotelManagementSystem.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection")
                       ?? throw new InvalidOperationException("Database connection string not found!");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Booking API",
        Version = "v1",
        Description = "An API for managing hotel bookings",
        Contact = new OpenApiContact
        {
            Name = "Stallone",
            Email = "support@mdeluxe.com",
            Url = new Uri("https://mdeluxe.com")
        }
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

//if (app.Environment.IsDevelopment()){
app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Booking API v1"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();

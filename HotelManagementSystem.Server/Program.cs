using HotelManagementSystem.Server.Controllers;
using HotelManagementSystem.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DB Connection
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection")
    ?? throw new InvalidOperationException("Database connection string not found!");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

//  Define a named CORS policy
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins(allowedOrigins ?? ["http://localhost:3000"]) // Angular default
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Optional: use only if sending cookies or auth headers
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireSecretaryRole", policy => policy.RequireRole("Secretary"))
    .AddPolicy("RequireManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));

builder.Services.AddSingleton<TokenService>();
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

    // Optional: Add JWT Auth support in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Use CORS with named policy
app.UseCors("CorsPolicy");

app.UseDefaultFiles();
app.MapStaticAssets();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Booking API v1"));

app.UseHttpsRedirection();
app.UseAuthentication(); // Ensure this is added before Authorization
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();

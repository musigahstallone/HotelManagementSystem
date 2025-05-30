﻿// <auto-generated />
using System;
using System.Collections.Generic;
using HotelManagementSystem.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HotelManagementSystem.Server.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20250327131526_postgresql_migration_554")]
partial class postgresql_migration_554
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.2")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Auth.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<int>("Role")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("Users");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Booking", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<DateTime?>("CancelledAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateOnly>("CheckInDate")
                    .HasColumnType("date");

                b.Property<DateOnly>("CheckOutDate")
                    .HasColumnType("date");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<bool>("IsCancelled")
                    .HasColumnType("boolean");

                b.Property<bool>("IsPaid")
                    .HasColumnType("boolean");

                b.Property<Guid>("RoomId")
                    .HasColumnType("uuid");

                b.Property<decimal>("TotalAmount")
                    .HasColumnType("numeric");

                b.Property<Guid>("UserId")
                    .HasColumnType("uuid");

                b.HasKey("Id");

                b.HasIndex("RoomId");

                b.HasIndex("UserId");

                b.ToTable("Bookings");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Hotel", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.PrimitiveCollection<List<string>>("Amenities")
                    .IsRequired()
                    .HasColumnType("text[]");

                b.Property<string>("ContactEmail")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("ContactPhone")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<bool>("IsActive")
                    .HasColumnType("boolean");

                b.Property<string>("Location")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("text");

                b.PrimitiveCollection<List<string>>("Policies")
                    .IsRequired()
                    .HasColumnType("text[]");

                b.Property<int>("StarRating")
                    .HasColumnType("integer");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Website")
                    .IsRequired()
                    .HasColumnType("text");

                b.HasKey("Id");

                b.HasIndex("Name")
                    .IsUnique();

                b.ToTable("Hotels");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Payment", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<decimal>("Amount")
                    .HasColumnType("numeric");

                b.Property<Guid>("BookingId")
                    .HasColumnType("uuid");

                b.Property<DateOnly>("PaymentDate")
                    .HasColumnType("date");

                b.Property<int>("Status")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("BookingId");

                b.ToTable("Payment");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Review", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("Comment")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<Guid>("HotelId")
                    .HasColumnType("uuid");

                b.Property<int>("Rating")
                    .HasColumnType("integer");

                b.Property<DateTime>("ReviewDate")
                    .HasColumnType("timestamp with time zone");

                b.Property<Guid>("UserId")
                    .HasColumnType("uuid");

                b.HasKey("Id");

                b.HasIndex("HotelId");

                b.HasIndex("UserId");

                b.ToTable("Review");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Room", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<int>("Capacity")
                    .HasColumnType("integer");

                b.Property<Guid>("HotelId")
                    .HasColumnType("uuid");

                b.Property<bool>("IsAvailable")
                    .HasColumnType("boolean");

                b.Property<decimal>("PricePerNight")
                    .HasColumnType("decimal(18,2)");

                b.Property<string>("RoomNumber")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<int>("Type")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("HotelId");

                b.ToTable("Rooms");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Todo.Todo", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<DateOnly?>("CompletedDate")
                    .HasColumnType("date");

                b.Property<TimeOnly?>("CompletedTime")
                    .HasColumnType("time");

                b.Property<DateOnly>("CreatedDate")
                    .HasColumnType("date");

                b.Property<TimeOnly>("CreatedTime")
                    .HasColumnType("time");

                b.Property<DateOnly?>("DueDate")
                    .HasColumnType("date");

                b.Property<bool>("IsArchived")
                    .HasColumnType("boolean");

                b.Property<bool>("IsCompleted")
                    .HasColumnType("boolean");

                b.Property<bool>("IsDeleted")
                    .HasColumnType("boolean");

                b.Property<string>("Note")
                    .HasColumnType("text");

                b.HasKey("Id");

                b.HasIndex("Note");

                b.ToTable("Todos");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Booking", b =>
            {
                b.HasOne("HotelManagementSystem.Server.Models.Hotels.Room", "Room")
                    .WithMany("Bookings")
                    .HasForeignKey("RoomId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("HotelManagementSystem.Server.Models.Auth.User", "User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Room");

                b.Navigation("User");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Payment", b =>
            {
                b.HasOne("HotelManagementSystem.Server.Models.Hotels.Booking", "Booking")
                    .WithMany()
                    .HasForeignKey("BookingId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Booking");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Review", b =>
            {
                b.HasOne("HotelManagementSystem.Server.Models.Hotels.Hotel", "Hotel")
                    .WithMany()
                    .HasForeignKey("HotelId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("HotelManagementSystem.Server.Models.Auth.User", "User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Hotel");

                b.Navigation("User");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Room", b =>
            {
                b.HasOne("HotelManagementSystem.Server.Models.Hotels.Hotel", "Hotel")
                    .WithMany("Rooms")
                    .HasForeignKey("HotelId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Hotel");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Hotel", b =>
            {
                b.Navigation("Rooms");
            });

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Hotels.Room", b =>
            {
                b.Navigation("Bookings");
            });
#pragma warning restore 612, 618
    }
}

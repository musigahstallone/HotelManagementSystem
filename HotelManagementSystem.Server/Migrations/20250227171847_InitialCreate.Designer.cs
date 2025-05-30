﻿// <auto-generated />
using System;
using HotelManagementSystem.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HotelManagementSystem.Server.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20250227171847_InitialCreate")]
partial class InitialCreate
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.2")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("HotelManagementSystem.Server.Models.Todo", b =>
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

                b.Property<bool>("IsCompleted")
                    .HasColumnType("boolean");

                b.Property<string>("Note")
                    .HasColumnType("text");

                b.HasKey("Id");

                b.ToTable("Todos");
            });
#pragma warning restore 612, 618
    }
}

﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementSystem.Server.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Todos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Note = table.Column<string>(type: "text", nullable: true),
                IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                CreatedTime = table.Column<TimeOnly>(type: "time", nullable: false),
                CompletedDate = table.Column<DateOnly>(type: "date", nullable: true),
                CompletedTime = table.Column<TimeOnly>(type: "time", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Todos", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Todos");
    }
}

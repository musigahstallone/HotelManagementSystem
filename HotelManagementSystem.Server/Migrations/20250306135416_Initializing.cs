using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementSystem.Server.Migrations;

/// <inheritdoc />
public partial class Initializing : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateOnly>(
            name: "DueDate",
            table: "Todos",
            type: "date",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsArchived",
            table: "Todos",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Todos",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DueDate",
            table: "Todos");

        migrationBuilder.DropColumn(
            name: "IsArchived",
            table: "Todos");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Todos");
    }
}

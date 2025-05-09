using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementSystem.Server.Migrations;

/// <inheritdoc />
public partial class AddedAlotOfAuthStaff : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "OtpCode",
            table: "Users",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateOnly>(
            name: "OtpExpiryDate",
            table: "Users",
            type: "date",
            nullable: true);

        migrationBuilder.AddColumn<TimeOnly>(
            name: "OtpExpiryTime",
            table: "Users",
            type: "time without time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "OtpCode",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "OtpExpiryDate",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "OtpExpiryTime",
            table: "Users");
    }
}

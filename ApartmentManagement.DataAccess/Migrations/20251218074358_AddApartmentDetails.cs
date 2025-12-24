using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddApartmentDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Apartments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Apartments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DefaultFlatType",
                table: "Apartments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultGrossArea",
                table: "Apartments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultMonthlyDue",
                table: "Apartments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultNetArea",
                table: "Apartments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FlatsPerFloor",
                table: "Apartments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasBalcony",
                table: "Apartments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasElevator",
                table: "Apartments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Apartments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Apartments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalFlats",
                table: "Apartments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalFloors",
                table: "Apartments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "DefaultFlatType",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "DefaultGrossArea",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "DefaultMonthlyDue",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "DefaultNetArea",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "FlatsPerFloor",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "HasBalcony",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "HasElevator",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "TotalFlats",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "TotalFloors",
                table: "Apartments");
        }
    }
}

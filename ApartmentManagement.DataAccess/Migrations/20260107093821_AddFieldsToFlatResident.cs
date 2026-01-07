using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToFlatResident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "FlatResidents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Rent",
                table: "FlatResidents",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FlatResidents",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "FlatResidents");

            migrationBuilder.DropColumn(
                name: "Rent",
                table: "FlatResidents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FlatResidents");
        }
    }
}

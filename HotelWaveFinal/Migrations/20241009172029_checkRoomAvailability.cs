using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelWaveFinal.Migrations
{
    /// <inheritdoc />
    public partial class checkRoomAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Bookings",
                newName: "CustomerName");

            migrationBuilder.AddColumn<int>(
                name: "MaxAdults",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxChildren",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAdults",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MaxChildren",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Bookings",
                newName: "Name");
        }
    }
}

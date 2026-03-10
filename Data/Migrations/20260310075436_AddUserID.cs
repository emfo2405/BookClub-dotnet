using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookClub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "averageRating",
                table: "Book");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Review",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Review");

            migrationBuilder.AddColumn<double>(
                name: "averageRating",
                table: "Book",
                type: "REAL",
                nullable: true);
        }
    }
}

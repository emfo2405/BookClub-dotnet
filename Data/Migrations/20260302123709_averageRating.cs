using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookClub.Data.Migrations
{
    /// <inheritdoc />
    public partial class averageRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "averageRating",
                table: "Book",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "averageRating",
                table: "Book");
        }
    }
}

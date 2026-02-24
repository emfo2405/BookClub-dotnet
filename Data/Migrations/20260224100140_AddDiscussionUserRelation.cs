using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookClub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscussionUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Discussion",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discussion_UserId",
                table: "Discussion",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discussion_AspNetUsers_UserId",
                table: "Discussion",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discussion_AspNetUsers_UserId",
                table: "Discussion");

            migrationBuilder.DropIndex(
                name: "IX_Discussion_UserId",
                table: "Discussion");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Discussion");
        }
    }
}

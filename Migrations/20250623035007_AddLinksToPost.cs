using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Click_Go.Migrations
{
    /// <inheritdoc />
    public partial class AddLinksToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkFacebook",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkGoogleMap",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkFacebook",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LinkGoogleMap",
                table: "Posts");
        }
    }
}

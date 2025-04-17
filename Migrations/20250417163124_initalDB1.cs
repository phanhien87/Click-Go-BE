using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Click_Go.Migrations
{
    /// <inheritdoc />
    public partial class initalDB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment_Id",
                table: "Images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Comment_Id",
                table: "Images",
                type: "bigint",
                nullable: true);
        }
    }
}

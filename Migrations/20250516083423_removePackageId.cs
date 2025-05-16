using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Click_Go.Migrations
{
    /// <inheritdoc />
    public partial class removePackageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PackageId",
                table: "UserPackages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPackages_PackageId",
                table: "UserPackages",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "ID");
        }
    }
}

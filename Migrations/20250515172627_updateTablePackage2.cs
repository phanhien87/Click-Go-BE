using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Click_Go.Migrations
{
    /// <inheritdoc />
    public partial class updateTablePackage2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Packages_PackageId1",
                table: "UserPackages");

            migrationBuilder.DropIndex(
                name: "IX_UserPackages_PackageId1",
                table: "UserPackages");

            migrationBuilder.DropColumn(
                name: "PackageId1",
                table: "UserPackages");

            migrationBuilder.AlterColumn<long>(
                name: "PackageId",
                table: "UserPackages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserPackages_PackageId",
                table: "UserPackages",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages");

            migrationBuilder.DropIndex(
                name: "IX_UserPackages_PackageId",
                table: "UserPackages");

            migrationBuilder.AlterColumn<int>(
                name: "PackageId",
                table: "UserPackages",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "PackageId1",
                table: "UserPackages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UserPackages_PackageId1",
                table: "UserPackages",
                column: "PackageId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Packages_PackageId1",
                table: "UserPackages",
                column: "PackageId1",
                principalTable: "Packages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

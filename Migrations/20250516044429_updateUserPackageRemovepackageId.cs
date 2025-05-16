using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Click_Go.Migrations
{
    /// <inheritdoc />
    public partial class updateUserPackageRemovepackageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa foreign key nếu tồn tại
            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages");

            // Xóa index liên quan đến PackageId
            migrationBuilder.DropIndex(
                name: "IX_UserPackages_PackageId",
                table: "UserPackages");

            // Xóa cột PackageId
            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "UserPackages");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Thêm lại cột PackageId
            migrationBuilder.AddColumn<long>(
                name: "PackageId",
                table: "UserPackages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            // Tạo lại index
            migrationBuilder.CreateIndex(
                name: "IX_UserPackages_PackageId",
                table: "UserPackages",
                column: "PackageId");

            // Thêm lại foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

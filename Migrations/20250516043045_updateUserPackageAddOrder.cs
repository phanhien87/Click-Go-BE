using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Click_Go.Migrations
{
    /// <inheritdoc />
    public partial class updateUserPackageAddOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "UserPackages");

            migrationBuilder.RenameColumn(
                name: "OrderCode",
                table: "UserPackages",
                newName: "OrderId");

            migrationBuilder.AlterColumn<long>(
                name: "PackageId",
                table: "UserPackages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_UserPackages_OrderId",
                table: "UserPackages",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Orders_OrderId",
                table: "UserPackages",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Orders_OrderId",
                table: "UserPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages");

            migrationBuilder.DropIndex(
                name: "IX_UserPackages_OrderId",
                table: "UserPackages");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "UserPackages",
                newName: "OrderCode");

            migrationBuilder.AlterColumn<long>(
                name: "PackageId",
                table: "UserPackages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Amount",
                table: "UserPackages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

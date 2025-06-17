using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Click_Go.Migrations
{
    /// <inheritdoc />
    public partial class AdđbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_Posts_PostId",
                table: "Voucher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher");

            migrationBuilder.RenameTable(
                name: "Voucher",
                newName: "Vouchers");

            migrationBuilder.RenameIndex(
                name: "IX_Voucher_PostId",
                table: "Vouchers",
                newName: "IX_Vouchers_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vouchers",
                table: "Vouchers",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Posts_PostId",
                table: "Vouchers",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Posts_PostId",
                table: "Vouchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vouchers",
                table: "Vouchers");

            migrationBuilder.RenameTable(
                name: "Vouchers",
                newName: "Voucher");

            migrationBuilder.RenameIndex(
                name: "IX_Vouchers_PostId",
                table: "Voucher",
                newName: "IX_Voucher_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_Posts_PostId",
                table: "Voucher",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

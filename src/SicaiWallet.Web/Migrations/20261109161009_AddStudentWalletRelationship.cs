using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SicaiWallet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentWalletRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WalletAccounts_StudentId",
                table: "WalletAccounts",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletAccounts_Students_StudentId",
                table: "WalletAccounts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletAccounts_Students_StudentId",
                table: "WalletAccounts");

            migrationBuilder.DropIndex(
                name: "IX_WalletAccounts_StudentId",
                table: "WalletAccounts");
        }
    }
}

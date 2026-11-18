using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SicaiWallet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundTransactionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginalTransactionId",
                table: "WalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_OriginalTransactionId",
                table: "WalletTransactions",
                column: "OriginalTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_WalletTransactions_OriginalTransactionId",
                table: "WalletTransactions",
                column: "OriginalTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_WalletTransactions_OriginalTransactionId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_OriginalTransactionId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "OriginalTransactionId",
                table: "WalletTransactions");
        }
    }
}

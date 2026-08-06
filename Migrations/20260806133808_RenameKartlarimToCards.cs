using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalBanking.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameKartlarimToCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kartlarim_Accounts_AccountId",
                table: "Kartlarim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kartlarim",
                table: "Kartlarim");

            migrationBuilder.RenameTable(
                name: "Kartlarim",
                newName: "Cards");

            migrationBuilder.RenameIndex(
                name: "IX_Kartlarim_CardNumber",
                table: "Cards",
                newName: "IX_Cards_CardNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Kartlarim_AccountId",
                table: "Cards",
                newName: "IX_Cards_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Accounts_AccountId",
                table: "Cards",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Accounts_AccountId",
                table: "Cards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "Kartlarim");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_CardNumber",
                table: "Kartlarim",
                newName: "IX_Kartlarim_CardNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_AccountId",
                table: "Kartlarim",
                newName: "IX_Kartlarim_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kartlarim",
                table: "Kartlarim",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Kartlarim_Accounts_AccountId",
                table: "Kartlarim",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

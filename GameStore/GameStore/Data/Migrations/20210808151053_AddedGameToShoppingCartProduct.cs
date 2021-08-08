using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddedGameToShoppingCartProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartProducts_GameId",
                table: "ShoppingCartProducts",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId",
                table: "ShoppingCartProducts",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId",
                table: "ShoppingCartProducts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartProducts_GameId",
                table: "ShoppingCartProducts");
        }
    }
}

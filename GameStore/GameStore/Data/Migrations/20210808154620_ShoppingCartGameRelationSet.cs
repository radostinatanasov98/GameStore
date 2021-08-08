using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class ShoppingCartGameRelationSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId",
                table: "ShoppingCartProducts");

            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "ShoppingCartProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartProducts_GameId1",
                table: "ShoppingCartProducts",
                column: "GameId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId",
                table: "ShoppingCartProducts",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId1",
                table: "ShoppingCartProducts",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId",
                table: "ShoppingCartProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartProducts_GameId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "ShoppingCartProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId",
                table: "ShoppingCartProducts",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

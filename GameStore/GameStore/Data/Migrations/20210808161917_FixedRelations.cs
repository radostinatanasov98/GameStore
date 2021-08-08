using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class FixedRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProducts_ShoppingCarts_ShoppingCartId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartProducts_GameId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartProducts_ShoppingCartId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId1",
                table: "ShoppingCartProducts");

            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartId2",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ShoppingCartId2",
                table: "Clients",
                column: "ShoppingCartId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_ShoppingCarts_ShoppingCartId2",
                table: "Clients",
                column: "ShoppingCartId2",
                principalTable: "ShoppingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ShoppingCarts_ShoppingCartId2",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_ShoppingCartId2",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId2",
                table: "Clients");

            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "ShoppingCartProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartId1",
                table: "ShoppingCartProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartProducts_GameId1",
                table: "ShoppingCartProducts",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartProducts_ShoppingCartId1",
                table: "ShoppingCartProducts",
                column: "ShoppingCartId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProducts_Games_GameId1",
                table: "ShoppingCartProducts",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProducts_ShoppingCarts_ShoppingCartId1",
                table: "ShoppingCartProducts",
                column: "ShoppingCartId1",
                principalTable: "ShoppingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

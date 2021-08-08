using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class ShoppingCardProductRelationsSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartId1",
                table: "ShoppingCartProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartProducts_ShoppingCartId1",
                table: "ShoppingCartProducts",
                column: "ShoppingCartId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProducts_ShoppingCarts_ShoppingCartId1",
                table: "ShoppingCartProducts",
                column: "ShoppingCartId1",
                principalTable: "ShoppingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProducts_ShoppingCarts_ShoppingCartId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartProducts_ShoppingCartId1",
                table: "ShoppingCartProducts");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId1",
                table: "ShoppingCartProducts");
        }
    }
}

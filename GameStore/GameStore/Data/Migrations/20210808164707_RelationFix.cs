using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class RelationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Clients_ClientId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_ClientId",
                table: "ShoppingCarts");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ShoppingCartId",
                table: "Clients",
                column: "ShoppingCartId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_ShoppingCarts_ShoppingCartId",
                table: "Clients",
                column: "ShoppingCartId",
                principalTable: "ShoppingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ShoppingCarts_ShoppingCartId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_ShoppingCartId",
                table: "Clients");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ClientId",
                table: "ShoppingCarts",
                column: "ClientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Clients_ClientId",
                table: "ShoppingCarts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

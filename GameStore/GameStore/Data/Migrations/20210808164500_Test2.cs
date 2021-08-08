using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class Test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ShoppingCarts_ShoppingCartId2",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Clients_ClientId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_Clients_ShoppingCartId2",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId2",
                table: "Clients");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Clients_ClientId",
                table: "ShoppingCarts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Clients_ClientId",
                table: "ShoppingCarts");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Clients_ClientId",
                table: "ShoppingCarts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

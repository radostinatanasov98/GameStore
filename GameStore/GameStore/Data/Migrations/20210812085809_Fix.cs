using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientRelationships_Clients_FriendId1",
                table: "ClientRelationships");

            migrationBuilder.DropIndex(
                name: "IX_ClientRelationships_FriendId1",
                table: "ClientRelationships");

            migrationBuilder.DropColumn(
                name: "FriendId1",
                table: "ClientRelationships");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRelationships_ClientId",
                table: "ClientRelationships",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRelationships_Clients_ClientId",
                table: "ClientRelationships",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientRelationships_Clients_ClientId",
                table: "ClientRelationships");

            migrationBuilder.DropIndex(
                name: "IX_ClientRelationships_ClientId",
                table: "ClientRelationships");

            migrationBuilder.AddColumn<int>(
                name: "FriendId1",
                table: "ClientRelationships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientRelationships_FriendId1",
                table: "ClientRelationships",
                column: "FriendId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRelationships_Clients_FriendId1",
                table: "ClientRelationships",
                column: "FriendId1",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

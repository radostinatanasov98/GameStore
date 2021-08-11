using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class ExtendedClientRelationshipTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AreFriends",
                table: "ClientRelationships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasFriendRequest",
                table: "ClientRelationships",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreFriends",
                table: "ClientRelationships");

            migrationBuilder.DropColumn(
                name: "HasFriendRequest",
                table: "ClientRelationships");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Publishers",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Clients",
                newName: "DisplayName");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Publishers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Clients",
                newName: "Name");
        }
    }
}

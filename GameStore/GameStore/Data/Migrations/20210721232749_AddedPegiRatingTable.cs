using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddedPegiRatingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PegiRating",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "PegiRatingId",
                table: "Games",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PegiRatingId1",
                table: "Games",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PegiRatings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PegiRatings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_PegiRatingId",
                table: "Games",
                column: "PegiRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PegiRatingId1",
                table: "Games",
                column: "PegiRatingId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_PegiRatings_PegiRatingId",
                table: "Games",
                column: "PegiRatingId",
                principalTable: "PegiRatings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_PegiRatings_PegiRatingId1",
                table: "Games",
                column: "PegiRatingId1",
                principalTable: "PegiRatings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_PegiRatings_PegiRatingId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_PegiRatings_PegiRatingId1",
                table: "Games");

            migrationBuilder.DropTable(
                name: "PegiRatings");

            migrationBuilder.DropIndex(
                name: "IX_Games_PegiRatingId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_PegiRatingId1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PegiRatingId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PegiRatingId1",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "PegiRating",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

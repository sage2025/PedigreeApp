using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddExtraValuesToStallionRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<double>(
                name: "CurrentIV",
                table: "StallionRating",
                nullable: true);
            migrationBuilder.AddColumn<double>(
                name: "CurrentAE",
                table: "StallionRating",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentPRB2",
                table: "StallionRating",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HistoricalIV",
                table: "StallionRating",
                nullable: true);
            migrationBuilder.AddColumn<double>(
                name: "HistoricalAE",
                table: "StallionRating",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HistoricalPRB2",
                table: "StallionRating",
                nullable: true);

        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAE",
                table: "StallionRating");

            migrationBuilder.DropColumn(
                name: "CurrentIV",
                table: "StallionRating");

            migrationBuilder.DropColumn(
                name: "CurrentPRB2",
                table: "StallionRating");

            migrationBuilder.DropColumn(
                name: "HistoricalAE",
                table: "StallionRating");

            migrationBuilder.DropColumn(
                name: "HistoricalIV",
                table: "StallionRating");

            migrationBuilder.DropColumn(
                name: "HistoricalPRB2",
                table: "StallionRating");

        }
    }
}

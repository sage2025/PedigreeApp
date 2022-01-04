using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddCoiFieldsToHorse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Coi",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi1",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi2",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi3",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi4",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi5",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi6",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi7",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi8",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coi",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi1",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi2",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi3",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi4",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi5",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi6",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi7",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Coi8",
                table: "Horse");
        }
    }
}

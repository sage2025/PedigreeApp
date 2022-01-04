using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddCalculationFieldsToHorse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Pedigcomp",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);
            migrationBuilder.AddColumn<double>(
                name: "Gi",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);
            migrationBuilder.AddColumn<double>(
                name: "Gdgs",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);
            migrationBuilder.AddColumn<double>(
                name: "Gdgd",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);
            migrationBuilder.AddColumn<double>(
                name: "Gssd",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);
            migrationBuilder.AddColumn<double>(
                name: "Gsdd",
                table: "Horse",
                nullable: false,
                defaultValue: 0.0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pedigcomp",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Gi",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Gdgs",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Gdgd",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Gssd",
                table: "Horse");
            migrationBuilder.DropColumn(
                name: "Gsdd",
                table: "Horse");
        }
    }
}

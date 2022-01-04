using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class RemoveParentsFieldsFromHorse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Father",
                table: "Horse"
            );

            migrationBuilder.DropColumn(
                name: "Mother",
                table: "Horse"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Father",
                table: "Horse",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Mother",
                table: "Horse",
                nullable: true
            );
        }

    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddCOIDFieldsToCoefficient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(name: "COID1", table: "Coefficient", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<double>(name: "COID2", table: "Coefficient", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<double>(name: "COID3", table: "Coefficient", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<double>(name: "COID4", table: "Coefficient", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<double>(name: "COID5", table: "Coefficient", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<double>(name: "COID6", table: "Coefficient", nullable: false, defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "COID1", table: "Coefficient");
            migrationBuilder.DropColumn(name: "COID2", table: "Coefficient");
            migrationBuilder.DropColumn(name: "COID3", table: "Coefficient");
            migrationBuilder.DropColumn(name: "COID4", table: "Coefficient");
            migrationBuilder.DropColumn(name: "COID5", table: "Coefficient");
            migrationBuilder.DropColumn(name: "COID6", table: "Coefficient");
        }
    }
}

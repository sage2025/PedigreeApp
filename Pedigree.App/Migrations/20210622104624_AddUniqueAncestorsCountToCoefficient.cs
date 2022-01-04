using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddUniqueAncestorsCountToCoefficient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniqueAncestorsCount",
                table: "Coefficient",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UniqueAncestorsCountUpdatedAt",
                table: "Coefficient",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueAncestorsCount",
                table: "Coefficient"); 
            
            migrationBuilder.DropColumn(
                 name: "UniqueAncestorsCountUpdatedAt",
                 table: "Coefficient");
        }
    }
}

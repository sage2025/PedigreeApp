using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddPedigreeColumnToPedigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pedigree",
                table: "Pedig",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PedigreeUpdatedAt",
                table: "Pedig",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pedigree",
                table: "Pedig"); 
            
            migrationBuilder.DropColumn(
                 name: "PedigreeUpdatedAt",
                 table: "Pedig");
        }
    }
}

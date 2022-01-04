using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddMtDNAColumnToHorse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MtDNA",
                table: "Horse",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Horse_MtDNA",
                table: "Horse",
                column: "MtDNA"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MtDNA",
                table: "Horse");
        }
    }
}

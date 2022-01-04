using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddSomeIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(name: "AncestorOId", table: "Ancestry", maxLength: 50);
            migrationBuilder.CreateIndex(
                name: "IX_Ancestry_AncestorOId",
                table: "Ancestry",
                column: "AncestorOId"
            );
            migrationBuilder.AlterColumn<string>(name: "HorseOId", table: "Pedig", maxLength: 50);
            migrationBuilder.CreateIndex(
                name: "IX_Pedig_HorseOId",
                table: "Pedig",
                column: "HorseOId"
            );
            migrationBuilder.CreateIndex(
                name: "IX_Position_Place",
                table: "Position",
                column: "Place"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}

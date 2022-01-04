using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class CreateInbreedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inbreed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InbreedOId = table.Column<string>(nullable: false, maxLength: 50),
                    OId = table.Column<string>(nullable: false, maxLength: 50),
                    SD = table.Column<string>(nullable: false),
                    Depth = table.Column<int>(nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inbreed", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inbreed_InbreedOId",
                table: "Inbreed",
                column: "InbreedOId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Inbreed_OId",
                table: "Inbreed",
                column: "OId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inbreed");
        }
    }
}

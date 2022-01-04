using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Pedigree.App.Migrations
{
    public partial class CreateMtDNAFlagTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MtDNAFlag",
                table: "Horse",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MtDNAFlag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartHorseOId = table.Column<string>(nullable: false),
                    EndHorseOId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MtDNAFlag", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MtDNAFlag",
                table: "Horse");

            migrationBuilder.DropTable(
                name: "MtDNAFlag");
        }
    }
}

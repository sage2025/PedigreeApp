using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddMLModelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MLModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HorsesCount = table.Column<int>(nullable: false),
                    ModelName = table.Column<string>(nullable: true),
                    RMSError = table.Column<double>(nullable: false),
                    RSquared = table.Column<double>(nullable: false),
                    ModelVersion = table.Column<int>(nullable: false),
                    ModelPath = table.Column<string>(nullable: true),
                    Features = table.Column<string>(nullable: true),
                    Deployed = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MLModels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MLModels");
        }
    }
}

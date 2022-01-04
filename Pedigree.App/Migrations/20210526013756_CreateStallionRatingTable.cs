using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pedigree.Infrastructure;

namespace Pedigree.App.Migrations
{
    public partial class CreateStallionRatingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StallionRating",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HorseOId = table.Column<string>(nullable: true, maxLength: 100),
                    CurrentRCount = table.Column<int>(nullable: true),
                    CurrentZCount = table.Column<int>(nullable: true),
                    CurrentStallionRating = table.Column<double>(nullable: true),
                    HistoricalRCount = table.Column<int>(nullable: true),
                    HistoricalZCount = table.Column<int>(nullable: true),
                    HistoricalStallionRating = table.Column<double>(nullable: true),
                    BMSCurrentRCount = table.Column<int>(nullable: true),
                    BMSCurrentZCount = table.Column<int>(nullable: true),
                    BMSCurrentStallionRating = table.Column<double>(nullable: true),
                    BMSHistoricalRCount = table.Column<int>(nullable: true),
                    BMSHistoricalZCount = table.Column<int>(nullable: true),
                    BMSHistoricalStallionRating = table.Column<double>(nullable: true),
                    SOSCurrentRCount = table.Column<int>(nullable: true),
                    SOSCurrentSCount = table.Column<int>(nullable: true),
                    SOSCurrentZCount = table.Column<int>(nullable: true),
                    SOSCurrentStallionRating = table.Column<double>(nullable: true),
                    SOSHistoricalRCount = table.Column<int>(nullable: true),
                    SOSHistoricalSCount = table.Column<int>(nullable: true),
                    SOSHistoricalZCount = table.Column<int>(nullable: true),
                    SOSHistoricalStallionRating = table.Column<double>(nullable: true),
                    BMSOSCurrentRCount = table.Column<int>(nullable: true),
                    BMSOSCurrentSCount = table.Column<int>(nullable: true),
                    BMSOSCurrentZCount = table.Column<int>(nullable: true),
                    BMSOSCurrentStallionRating = table.Column<double>(nullable: true),
                    BMSOSHistoricalRCount = table.Column<int>(nullable: true),
                    BMSOSHistoricalSCount = table.Column<int>(nullable: true),
                    BMSOSHistoricalZCount = table.Column<int>(nullable: true),
                    BMSOSHistoricalStallionRating = table.Column<double>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StallionRating", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StallionRating_HorseOId",
                table: "StallionRating",
                column: "HorseOId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StallionRating");
        }
    }
}

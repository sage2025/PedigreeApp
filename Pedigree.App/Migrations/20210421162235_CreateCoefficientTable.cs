using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class CreateCoefficientTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coefficient",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HorseOId = table.Column<string>(nullable: false, maxLength: 50),
                    COI = table.Column<double>(nullable: false, defaultValue: 0),
                    COI1 = table.Column<double>(nullable: false, defaultValue: 0),
                    COI2 = table.Column<double>(nullable: false, defaultValue: 0),
                    COI3 = table.Column<double>(nullable: false, defaultValue: 0),
                    COI4 = table.Column<double>(nullable: false, defaultValue: 0),
                    COI5 = table.Column<double>(nullable: false, defaultValue: 0),
                    COI6 = table.Column<double>(nullable: false, defaultValue: 0),
                    COI7 = table.Column<double>(nullable: false, defaultValue: 0),
                    COI8 = table.Column<double>(nullable: false, defaultValue: 0),
                    Pedigcomp = table.Column<double>(nullable: false, defaultValue: 0),
                    GI = table.Column<double>(nullable: false, defaultValue: 0),
                    GDGS = table.Column<double>(nullable: false, defaultValue: 0),
                    GDGD = table.Column<double>(nullable: false, defaultValue: 0),
                    GSSD = table.Column<double>(nullable: false, defaultValue: 0),
                    GSDD = table.Column<double>(nullable: false, defaultValue: 0),
                    COIUpdatedAt = table.Column<DateTime>(nullable: true),
                    HistoricalBPR = table.Column<double>(nullable: true),
                    HistoricalRD = table.Column<double>(nullable: true),
                    ZHistoricalBPR = table.Column<double>(nullable: true),
                    CurrentBPR = table.Column<double>(nullable: true),
                    CurrentRD = table.Column<double>(nullable: true),
                    ZCurrentBPR = table.Column<double>(nullable: true),
                    BPRUpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: DateTime.UtcNow)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coefficient", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coeficient_HorseOId",
                table: "Coefficient",
                column: "HorseOId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Coeficient_COIUpdatedAt",
                table: "Coefficient",
                column: "COIUpdatedAt"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Coeficient_BPRUpdatedAt",
                table: "Coefficient",
                column: "BPRUpdatedAt"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Coeficient_Pedigcomp",
                table: "Coefficient",
                column: "Pedigcomp"
            );

            migrationBuilder.Sql(
                "INSERT INTO Coefficient (HorseOId, COI, COI1, COI2, COI3, COI4, COI5, COI6, COI7, COI8, Pedigcomp, GI, GDGS, GDGD, GSSD, GSDD, COIUpdatedAt) " +
                "SELECT OId, Coi, Coi1, Coi2, Coi3, Coi4, Coi5, Coi6, Coi7, Coi8, Pedigcomp, Gi, Gdgs, Gdgd, Gssd, Gsdd, SyncedAt FROM Horse WHERE SyncedAt IS NOT NULL"
            );

            migrationBuilder.DropIndex(
                name: "IX_Horse_Pedigcomp",
                table: "Horse"
            );

            migrationBuilder.DropColumn(
                name: "Coi",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi1",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi2",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi3",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi4",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi5",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi6",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi7",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Coi8",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Gdgd",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Gdgs",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Gi",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Gsdd",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Gssd",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "Pedigcomp",
                table: "Horse");

            migrationBuilder.DropColumn(
                name: "SyncedAt",
                table: "Horse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coefficient");

            migrationBuilder.AddColumn<double>(
                name: "Coi",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi1",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi2",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi3",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi4",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi5",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi6",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi7",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coi8",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Gdgd",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Gdgs",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Gi",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Gsdd",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Gssd",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Pedigcomp",
                table: "Horse",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncedAt",
                table: "Horse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Horse_Pedigcomp",
                table: "Horse",
                column: "Pedigcomp"
            );

        }
    }
}

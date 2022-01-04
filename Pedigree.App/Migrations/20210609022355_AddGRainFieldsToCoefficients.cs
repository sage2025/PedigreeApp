using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class AddGRainFieldsToCoefficients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "GRainProcessStartedAt",
                table: "Coefficient",
                nullable: true);
            
            migrationBuilder.AddColumn<double>(
                name: "Bal",
                table: "Coefficient",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AHC",
                table: "Coefficient",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Kal",
                table: "Coefficient",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GRainUpdatedAt",
                table: "Coefficient",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "GRainProcessStartedAt",
                table: "Coefficient");

            migrationBuilder.DropColumn(
                name: "AHC",
                table: "Coefficient");

            migrationBuilder.DropColumn(
                name: "Bal",
                table: "Coefficient");

            migrationBuilder.DropColumn(
                name: "Kal",
                table: "Coefficient");

            migrationBuilder.DropColumn(
                name: "GRainUpdatedAt",
                table: "Coefficient");
        }
    }
}

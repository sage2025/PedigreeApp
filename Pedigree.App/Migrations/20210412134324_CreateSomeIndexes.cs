using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class CreateSomeIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Horse Indexes
            migrationBuilder.CreateIndex(
                name: "IX_Horse_Pedigcomp",
                table: "Horse",
                column: "Pedigcomp"
            );

            // 2. Race Indexes
            migrationBuilder.AlterColumn<string>(name:"Country", table:"Race", maxLength: 10);
            migrationBuilder.CreateIndex(
                name: "IX_Race_Date",
                table: "Race",
                column: "Date"
            );
            migrationBuilder.CreateIndex(
                name: "IX_Race_Country",
                table: "Race",
                column: "Country"
            );

            migrationBuilder.AlterColumn<string>(name: "Distance", table: "Race", maxLength: 30);
            migrationBuilder.CreateIndex(
                name: "IX_Race_Distance",
                table: "Race",
                column: "Distance"
            );
            migrationBuilder.AlterColumn<string>(name: "Surface", table: "Race", maxLength: 30);
            migrationBuilder.CreateIndex(
                name: "IX_Race_Surface",
                table: "Race",
                column: "Surface"
            );

            migrationBuilder.AlterColumn<string>(name: "Type", table: "Race", maxLength: 30);
            migrationBuilder.CreateIndex(
                name: "IX_Race_Type",
                table: "Race",
                column: "Type"
            );

            migrationBuilder.AlterColumn<string>(name: "Status", table: "Race", maxLength: 10);
            migrationBuilder.CreateIndex(
                name: "IX_Race_Status",
                table: "Race",
                column: "Status"
            );

            // 3. Weight Indexes
            migrationBuilder.AlterColumn<string>(name: "Country", table: "Weight", maxLength: 10);
            migrationBuilder.CreateIndex(
                name: "IX_Weight_Country",
                table: "Weight",
                column: "Country"
            );
            migrationBuilder.AlterColumn<string>(name: "Distance", table: "Weight", maxLength: 30);
            migrationBuilder.CreateIndex(
                name: "IX_Weight_Distance",
                table: "Weight",
                column: "Distance"
            );
            migrationBuilder.AlterColumn<string>(name: "Surface", table: "Weight", maxLength: 30);
            migrationBuilder.CreateIndex(
                name: "IX_Weight_Surface",
                table: "Weight",
                column: "Surface"
            );
            migrationBuilder.AlterColumn<string>(name: "Type", table: "Weight", maxLength: 10);
            migrationBuilder.CreateIndex(
                name: "IX_Weight_Type",
                table: "Weight",
                column: "Type"
            );
            migrationBuilder.AlterColumn<string>(name: "Status", table: "Weight", maxLength: 10);
            migrationBuilder.CreateIndex(
                name: "IX_Weight_Status",
                table: "Weight",
                column: "Status"
            );

            // 4. Position Indexes
            migrationBuilder.AlterColumn<string>(name: "HorseOId", table: "Position", maxLength: 50);
            migrationBuilder.CreateIndex(
                name: "IX_Position_HorseOId",
                table: "Position",
                column: "HorseOId"
            );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Horse Index
            migrationBuilder.DropIndex(
                name: "IX_Horse_Pedigcomp",
                table: "Horse"
            );

            // 2. Race Indexes
            migrationBuilder.DropIndex(
                name: "IX_Race_Date",
                table: "Race"
            );
            migrationBuilder.DropIndex(
                name: "IX_Race_Country",
                table: "Race"
            );
            migrationBuilder.DropIndex(
                name: "IX_Race_Distance",
                table: "Race"
            );
            migrationBuilder.DropIndex(
                name: "IX_Race_Surface",
                table: "Race"
            );
            migrationBuilder.DropIndex(
                name: "IX_Race_Type",
                table: "Race"
            );
            migrationBuilder.DropIndex(
                name: "IX_Race_Status",
                table: "Race"
            );

            // 3. Weight Indexes
            migrationBuilder.DropIndex(
                name: "IX_Weight_Country",
                table: "Weight"
            );
            migrationBuilder.DropIndex(
                name: "IX_Weight_Distance",
                table: "Weight"
            );
            migrationBuilder.DropIndex(
                name: "IX_Weight_Surface",
                table: "Weight"
            );
            migrationBuilder.DropIndex(
                name: "IX_Weight_Type",
                table: "Weight"
            );
            migrationBuilder.DropIndex(
                name: "IX_Weight_Status",
                table: "Weight"
            );

            // 4. Position Indexes
            /*migrationBuilder.DropIndex(
                name: "IX_Position_HorseOId",
                table: "Position"
            );*/
            migrationBuilder.DropForeignKey(
                name: "FK_Position_Race",
                table: "Position"
            );
        }
    }
}

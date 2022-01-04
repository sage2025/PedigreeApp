using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pedigree.App.Migrations
{
    public partial class CreateHaploGroupTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HaploGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: false, maxLength: 10),
                    Color = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: DateTime.UtcNow)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HaploGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HaploType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false, maxLength: 10),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: DateTime.UtcNow)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HaploType", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_HaploType_HaploGroup",
                table: "HaploType",
                column: "GroupId",
                principalTable: "HaploGroup",
                principalColumn: "Id"
              );

            var groupTitles = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R" };
            var groupColors = new Dictionary<string, string>();
            foreach(var t in groupTitles)
            {
                if (t.Equals("L")) groupColors[t] = "#e6194B";
                else if (t.Equals("N")) groupColors[t] = "#3cb44b";
                else if (t.Equals("I")) groupColors[t] = "#ffe119";
                else if (t.Equals("G")) groupColors[t] = "#4363d8";
                else groupColors[t] = "";
            }
            string sql = $"INSERT INTO HaploGroup (Title, Color) VALUES {string.Join(",", groupTitles.Select(t => string.Format("('{0}', '{1}')", t, groupColors[t])))}";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HaploGroup"
            );
            migrationBuilder.DropTable(
                name: "HaploType"
            );

        }
    }
}

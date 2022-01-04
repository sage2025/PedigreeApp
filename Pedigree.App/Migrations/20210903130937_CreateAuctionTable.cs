using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Pedigree.App.Migrations
{
    public partial class CreateAuctionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuctionName = table.Column<string>(nullable: false),
                    AuctionDate = table.Column<string>(nullable: false),
                    AuctionType = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuctionDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuctionId = table.Column<int>(nullable: false),
                    LotNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    YOB = table.Column<int>(nullable: false),
                    Sex = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    SireId = table.Column<int>(nullable: false),
                    DamId = table.Column<int>(nullable: false),
                    mtDNAHapId = table.Column<int>(nullable: false),
                    mlScore = table.Column<double>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionDetail", x => x.Id);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auction");
            migrationBuilder.DropTable(
                name: "AuctionDetail");
        }
    }
}

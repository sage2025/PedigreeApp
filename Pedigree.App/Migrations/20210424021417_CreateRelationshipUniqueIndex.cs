using Microsoft.EntityFrameworkCore.Migrations;

namespace Pedigree.App.Migrations
{
    public partial class CreateRelationshipUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(name: "HorseOId", table: "Relationship", maxLength: 50);
            migrationBuilder.AlterColumn<string>(name: "ParentType", table: "Relationship", maxLength: 10);
            migrationBuilder.CreateIndex(
                name: "IX_Relationship_Parent_Unique",
                table: "Relationship",
                columns: new string[] { "HorseOId", "ParentType"},
                unique: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Relationship_Parent_Unique",
                table: "Relationship"
            );
        }
    }
}

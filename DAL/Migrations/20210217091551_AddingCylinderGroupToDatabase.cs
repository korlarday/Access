using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingCylinderGroupToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CylinderGroups",
                columns: table => new
                {
                    CylinderID = table.Column<int>(nullable: false),
                    GroupID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CylinderGroups", x => new { x.CylinderID, x.GroupID });
                    table.ForeignKey(
                        name: "FK_CylinderGroups_Cylinders_CylinderID",
                        column: x => x.CylinderID,
                        principalTable: "Cylinders",
                        principalColumn: "CylinderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CylinderGroups_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CylinderGroups_GroupID",
                table: "CylinderGroups",
                column: "GroupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CylinderGroups");
        }
    }
}

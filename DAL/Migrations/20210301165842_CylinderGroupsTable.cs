using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class CylinderGroupsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CylinderGroupsRelations",
                columns: table => new
                {
                    CylinderGroupsRelationID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    _CylinderID = table.Column<string>(nullable: true),
                    _DoorName = table.Column<string>(nullable: true),
                    _CylinderNumber = table.Column<string>(nullable: true),
                    _GroupId = table.Column<string>(nullable: true),
                    _GroupName = table.Column<string>(nullable: true),
                    _RelatedGrouping = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CylinderGroupsRelations", x => x.CylinderGroupsRelationID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CylinderGroupsRelations");
        }
    }
}

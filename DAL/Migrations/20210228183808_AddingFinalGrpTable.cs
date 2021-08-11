using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingFinalGrpTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherValues",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "SubGroupItems",
                table: "GroupSummaries");

            migrationBuilder.AddColumn<string>(
                name: "Cylinders",
                table: "GroupSummaries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerGroups",
                table: "GroupSummaries",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupFinals",
                columns: table => new
                {
                    GroupFinalID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GroupID = table.Column<int>(nullable: false),
                    RelatedGrouping = table.Column<string>(nullable: true),
                    Validated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupFinals", x => x.GroupFinalID);
                    table.ForeignKey(
                        name: "FK_GroupFinals_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupFinals_GroupID",
                table: "GroupFinals",
                column: "GroupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "Cylinders",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "InnerGroups",
                table: "GroupSummaries");

            migrationBuilder.AddColumn<string>(
                name: "OtherValues",
                table: "GroupSummaries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubGroupItems",
                table: "GroupSummaries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}

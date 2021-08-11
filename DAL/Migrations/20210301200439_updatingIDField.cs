using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class updatingIDField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_KeyGroupCylinderAnalyses",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupSummaries",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GroupSummaries");

            migrationBuilder.AddColumn<int>(
                name: "KeyGroupCylinderAnalysisID",
                table: "KeyGroupCylinderAnalyses",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "GroupSummaryID",
                table: "GroupSummaries",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeyGroupCylinderAnalyses",
                table: "KeyGroupCylinderAnalyses",
                column: "KeyGroupCylinderAnalysisID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupSummaries",
                table: "GroupSummaries",
                column: "GroupSummaryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_KeyGroupCylinderAnalyses",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupSummaries",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "KeyGroupCylinderAnalysisID",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropColumn(
                name: "GroupSummaryID",
                table: "GroupSummaries");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "KeyGroupCylinderAnalyses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GroupSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeyGroupCylinderAnalyses",
                table: "KeyGroupCylinderAnalyses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupSummaries",
                table: "GroupSummaries",
                column: "Id");
        }
    }
}

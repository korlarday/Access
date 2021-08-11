using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class IndexingOnTabls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cylinders",
                table: "KeyGroupCylinderDetails");

            migrationBuilder.DropColumn(
                name: "KeyGroup",
                table: "KeyGroupCylinderDetails");

            migrationBuilder.DropColumn(
                name: "Cylinder",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropColumn(
                name: "GroupKey",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropColumn(
                name: "NumOfMatches",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "RelatedGrouping",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "Validated",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "_CylinderID",
                table: "CylinderGroupsRelations");

            migrationBuilder.DropColumn(
                name: "_CylinderNumber",
                table: "CylinderGroupsRelations");

            migrationBuilder.DropColumn(
                name: "_DoorName",
                table: "CylinderGroupsRelations");

            migrationBuilder.DropColumn(
                name: "_GroupId",
                table: "CylinderGroupsRelations");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "KeyGroupCylinderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "KeyGroupCylinderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "_Cylinders",
                table: "KeyGroupCylinderDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "KeyGroupCylinderAnalyses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "_Cylinder",
                table: "KeyGroupCylinderAnalyses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "GroupSummaries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "GroupFinals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_NumOfMatches",
                table: "GroupFinals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "_RelatedGrouping",
                table: "GroupFinals",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "_Validated",
                table: "GroupFinals",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CylinderID",
                table: "CylinderGroupsRelations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "CylinderGroupsRelations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_KeyGroupCylinderDetails_CustomerID",
                table: "KeyGroupCylinderDetails",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupFinals_CustomerID",
                table: "GroupFinals",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupFinals_Customers_CustomerID",
                table: "GroupFinals",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyGroupCylinderDetails_Customers_CustomerID",
                table: "KeyGroupCylinderDetails",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupFinals_Customers_CustomerID",
                table: "GroupFinals");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyGroupCylinderDetails_Customers_CustomerID",
                table: "KeyGroupCylinderDetails");

            migrationBuilder.DropIndex(
                name: "IX_KeyGroupCylinderDetails_CustomerID",
                table: "KeyGroupCylinderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GroupFinals_CustomerID",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "KeyGroupCylinderDetails");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "KeyGroupCylinderDetails");

            migrationBuilder.DropColumn(
                name: "_Cylinders",
                table: "KeyGroupCylinderDetails");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropColumn(
                name: "_Cylinder",
                table: "KeyGroupCylinderAnalyses");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "_NumOfMatches",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "_RelatedGrouping",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "_Validated",
                table: "GroupFinals");

            migrationBuilder.DropColumn(
                name: "CylinderID",
                table: "CylinderGroupsRelations");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "CylinderGroupsRelations");

            migrationBuilder.AddColumn<string>(
                name: "Cylinders",
                table: "KeyGroupCylinderDetails",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyGroup",
                table: "KeyGroupCylinderDetails",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cylinder",
                table: "KeyGroupCylinderAnalyses",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupKey",
                table: "KeyGroupCylinderAnalyses",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfMatches",
                table: "GroupFinals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RelatedGrouping",
                table: "GroupFinals",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Validated",
                table: "GroupFinals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "_CylinderID",
                table: "CylinderGroupsRelations",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "_CylinderNumber",
                table: "CylinderGroupsRelations",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "_DoorName",
                table: "CylinderGroupsRelations",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "_GroupId",
                table: "CylinderGroupsRelations",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}

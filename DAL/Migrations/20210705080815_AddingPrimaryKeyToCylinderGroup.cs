using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingPrimaryKeyToCylinderGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CylinderGroupVerifications",
                table: "CylinderGroupVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CylinderGroups",
                table: "CylinderGroups");

            migrationBuilder.AddColumn<int>(
                name: "CylinderGroupVerificationID",
                table: "CylinderGroupVerifications",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "CylinderGroupID",
                table: "CylinderGroups",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CylinderGroupVerifications",
                table: "CylinderGroupVerifications",
                column: "CylinderGroupVerificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CylinderGroups",
                table: "CylinderGroups",
                column: "CylinderGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_CylinderGroupVerifications_CylinderID",
                table: "CylinderGroupVerifications",
                column: "CylinderID");

            migrationBuilder.CreateIndex(
                name: "IX_CylinderGroups_CylinderID",
                table: "CylinderGroups",
                column: "CylinderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CylinderGroupVerifications",
                table: "CylinderGroupVerifications");

            migrationBuilder.DropIndex(
                name: "IX_CylinderGroupVerifications_CylinderID",
                table: "CylinderGroupVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CylinderGroups",
                table: "CylinderGroups");

            migrationBuilder.DropIndex(
                name: "IX_CylinderGroups_CylinderID",
                table: "CylinderGroups");

            migrationBuilder.DropColumn(
                name: "CylinderGroupVerificationID",
                table: "CylinderGroupVerifications");

            migrationBuilder.DropColumn(
                name: "CylinderGroupID",
                table: "CylinderGroups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CylinderGroupVerifications",
                table: "CylinderGroupVerifications",
                columns: new[] { "CylinderID", "GroupID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CylinderGroups",
                table: "CylinderGroups",
                columns: new[] { "CylinderID", "GroupID" });
        }
    }
}

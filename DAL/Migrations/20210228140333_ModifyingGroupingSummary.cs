using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class ModifyingGroupingSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_Cylinders",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "_KeyGroup",
                table: "GroupSummaries");

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "GroupSummaries",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubGroup",
                table: "GroupSummaries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OtherValues",
                table: "GroupSummaries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubGroupItems",
                table: "GroupSummaries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubGroupName",
                table: "GroupSummaries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "IsSubGroup",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "OtherValues",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "SubGroupItems",
                table: "GroupSummaries");

            migrationBuilder.DropColumn(
                name: "SubGroupName",
                table: "GroupSummaries");

            migrationBuilder.AddColumn<string>(
                name: "_Cylinders",
                table: "GroupSummaries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "_KeyGroup",
                table: "GroupSummaries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddedReclaimedToProduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_Pending",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "_Produced",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "_Summary",
                table: "Productions");

            migrationBuilder.AddColumn<string>(
                name: "ByUserId",
                table: "Productions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "_Reclaimed",
                table: "Groups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_Cylinder",
                table: "Cylinders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Productions_ByUserId",
                table: "Productions",
                column: "ByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedById",
                table: "Orders",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedById",
                table: "Orders",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productions_AspNetUsers_ByUserId",
                table: "Productions",
                column: "ByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedById",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Productions_AspNetUsers_ByUserId",
                table: "Productions");

            migrationBuilder.DropIndex(
                name: "IX_Productions_ByUserId",
                table: "Productions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ByUserId",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "_Reclaimed",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "_Cylinder",
                table: "Cylinders");

            migrationBuilder.AddColumn<int>(
                name: "_Pending",
                table: "Productions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_Produced",
                table: "Productions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "_Summary",
                table: "Productions",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}

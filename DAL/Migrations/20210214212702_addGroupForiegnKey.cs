using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class addGroupForiegnKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GroupID",
                table: "Orders",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Groups_GroupID",
                table: "Orders",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Groups_GroupID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_GroupID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "Orders");
        }
    }
}

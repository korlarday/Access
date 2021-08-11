using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddedUserNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "_UserNumber",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PartnerID",
                table: "AspNetUsers",
                column: "PartnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Partners_PartnerID",
                table: "AspNetUsers",
                column: "PartnerID",
                principalTable: "Partners",
                principalColumn: "PartnerID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Partners_PartnerID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PartnerID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "_UserNumber",
                table: "AspNetUsers");
        }
    }
}

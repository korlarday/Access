using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddEmailToCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "_Email",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "_EmailVerified",
                table: "Customers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "_EmailVerified",
                table: "Customers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingStatusToCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "_CustomerStatus",
                table: "Customers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "_IsDeleted",
                table: "Customers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_CustomerStatus",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "_IsDeleted",
                table: "Customers");
        }
    }
}

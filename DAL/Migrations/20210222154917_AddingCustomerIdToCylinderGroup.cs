using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingCustomerIdToCylinderGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "CylinderGroups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CylinderGroups_CustomerID",
                table: "CylinderGroups",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CylinderGroups_Customers_CustomerID",
                table: "CylinderGroups",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CylinderGroups_Customers_CustomerID",
                table: "CylinderGroups");

            migrationBuilder.DropIndex(
                name: "IX_CylinderGroups_CustomerID",
                table: "CylinderGroups");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "CylinderGroups");
        }
    }
}

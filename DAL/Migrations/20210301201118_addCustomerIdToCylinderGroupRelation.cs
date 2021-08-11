using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class addCustomerIdToCylinderGroupRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "CylinderGroupsRelations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CylinderGroupsRelations_CustomerID",
                table: "CylinderGroupsRelations",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CylinderGroupsRelations_Customers_CustomerID",
                table: "CylinderGroupsRelations",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CylinderGroupsRelations_Customers_CustomerID",
                table: "CylinderGroupsRelations");

            migrationBuilder.DropIndex(
                name: "IX_CylinderGroupsRelations_CustomerID",
                table: "CylinderGroupsRelations");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "CylinderGroupsRelations");
        }
    }
}

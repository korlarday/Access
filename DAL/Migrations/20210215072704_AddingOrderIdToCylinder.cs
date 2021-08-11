using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingOrderIdToCylinder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cylinders_CylinderID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Groups_GroupID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CylinderID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_GroupID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CylinderID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "OperatorId",
                table: "SystemAudits",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "_ProductID",
                table: "Productions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_Status",
                table: "Productions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_CylinderQuantity",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_KeyQuantity",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Groups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "Groups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Cylinders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "Cylinders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SystemAudits_OperatorId",
                table: "SystemAudits",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CustomerID",
                table: "Groups",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OrderID",
                table: "Groups",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Cylinders_CustomerID",
                table: "Cylinders",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Cylinders_OrderID",
                table: "Cylinders",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cylinders_Customers_CustomerID",
                table: "Cylinders",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cylinders_Orders_OrderID",
                table: "Cylinders",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Customers_CustomerID",
                table: "Groups",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Orders_OrderID",
                table: "Groups",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAudits_AspNetUsers_OperatorId",
                table: "SystemAudits",
                column: "OperatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cylinders_Customers_CustomerID",
                table: "Cylinders");

            migrationBuilder.DropForeignKey(
                name: "FK_Cylinders_Orders_OrderID",
                table: "Cylinders");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Customers_CustomerID",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Orders_OrderID",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemAudits_AspNetUsers_OperatorId",
                table: "SystemAudits");

            migrationBuilder.DropIndex(
                name: "IX_SystemAudits_OperatorId",
                table: "SystemAudits");

            migrationBuilder.DropIndex(
                name: "IX_Groups_CustomerID",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_OrderID",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Cylinders_CustomerID",
                table: "Cylinders");

            migrationBuilder.DropIndex(
                name: "IX_Cylinders_OrderID",
                table: "Cylinders");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                table: "SystemAudits");

            migrationBuilder.DropColumn(
                name: "_ProductID",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "_Status",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "_CylinderQuantity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "_KeyQuantity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Cylinders");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "Cylinders");

            migrationBuilder.AddColumn<int>(
                name: "CylinderID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CylinderID",
                table: "Orders",
                column: "CylinderID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GroupID",
                table: "Orders",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Cylinders_CylinderID",
                table: "Orders",
                column: "CylinderID",
                principalTable: "Cylinders",
                principalColumn: "CylinderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Groups_GroupID",
                table: "Orders",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class UpdateDatabaseModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Productions_ProductionID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductionID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductionID",
                table: "OrderDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductionID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductionID",
                table: "OrderDetails",
                column: "ProductionID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Productions_ProductionID",
                table: "OrderDetails",
                column: "ProductionID",
                principalTable: "Productions",
                principalColumn: "ProductionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

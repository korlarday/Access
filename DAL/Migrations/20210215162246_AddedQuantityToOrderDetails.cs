using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddedQuantityToOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "_NewQty",
                table: "OrderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "_Notes",
                table: "OrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "_OldQty",
                table: "OrderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_ProductID",
                table: "OrderDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_NewQty",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "_Notes",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "_OldQty",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "_ProductID",
                table: "OrderDetails");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class CorrectingModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "_CylinderQuantity",
            //    table: "Orders");

            //migrationBuilder.DropColumn(
            //    name: "_KeyQuantity",
            //    table: "Orders");

            //migrationBuilder.DropColumn(
            //    name: "_Cylinder",
            //    table: "Cylinders");

            migrationBuilder.AlterColumn<string>(
                name: "_OrderNumber",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "_CylinderQty",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_GroupKeyQty",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "_KeyNumber",
                table: "Groups",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_CylinderQty",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "_GroupKeyQty",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "_OrderNumber",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "_CylinderQuantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "_KeyQuantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "_KeyNumber",
                table: "Groups",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "_Cylinder",
                table: "Cylinders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

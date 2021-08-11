using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingCylinderGroupVerifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "CylinderGroupVerifications",
            //    columns: table => new
            //    {
            //        CylinderID = table.Column<int>(nullable: false),
            //        GroupID = table.Column<int>(nullable: false),
            //        CustomerID = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CylinderGroupVerifications", x => new { x.CylinderID, x.GroupID });
            //        table.ForeignKey(
            //            name: "FK_CylinderGroupVerifications_Customers_CustomerID",
            //            column: x => x.CustomerID,
            //            principalTable: "Customers",
            //            principalColumn: "CustomerID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_CylinderGroupVerifications_Cylinders_CylinderID",
            //            column: x => x.CylinderID,
            //            principalTable: "Cylinders",
            //            principalColumn: "CylinderID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_CylinderGroupVerifications_Groups_GroupID",
            //            column: x => x.GroupID,
            //            principalTable: "Groups",
            //            principalColumn: "GroupID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_CylinderGroupVerifications_CustomerID",
            //    table: "CylinderGroupVerifications",
            //    column: "CustomerID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CylinderGroupVerifications_GroupID",
            //    table: "CylinderGroupVerifications",
            //    column: "GroupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CylinderGroupVerifications");
        }
    }
}

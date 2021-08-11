using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddConfigurationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    ConfigurationID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sapengineuri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.ConfigurationID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productions_OrderID",
                table: "Productions",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Productions_Orders_OrderID",
                table: "Productions",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productions_Orders_OrderID",
                table: "Productions");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropIndex(
                name: "IX_Productions_OrderID",
                table: "Productions");
        }
    }
}

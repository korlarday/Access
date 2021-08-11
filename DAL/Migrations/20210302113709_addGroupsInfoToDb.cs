using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class addGroupsInfoToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Discs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GroupsInfos",
                columns: table => new
                {
                    GroupsInfoID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    _Slot = table.Column<int>(nullable: false),
                    _Row = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    GroupID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsInfos", x => x.GroupsInfoID);
                    table.ForeignKey(
                        name: "FK_GroupsInfos_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsInfos_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discs_CustomerID",
                table: "Discs",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsInfos_CustomerID",
                table: "GroupsInfos",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsInfos_GroupID",
                table: "GroupsInfos",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Discs_Customers_CustomerID",
                table: "Discs",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discs_Customers_CustomerID",
                table: "Discs");

            migrationBuilder.DropTable(
                name: "GroupsInfos");

            migrationBuilder.DropIndex(
                name: "IX_Discs_CustomerID",
                table: "Discs");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Discs");
        }
    }
}

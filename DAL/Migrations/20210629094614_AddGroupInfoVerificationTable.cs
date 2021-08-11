using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddGroupInfoVerificationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupInfoVerifications",
                columns: table => new
                {
                    GroupInfoVerificationID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    _Slot = table.Column<int>(nullable: false),
                    _Row = table.Column<int>(nullable: false),
                    _Value = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    GroupID = table.Column<int>(nullable: false),
                    _CreationDate = table.Column<DateTime>(nullable: false),
                    _UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInfoVerifications", x => x.GroupInfoVerificationID);
                    table.ForeignKey(
                        name: "FK_GroupInfoVerifications_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupInfoVerifications_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupInfoVerifications_CustomerID",
                table: "GroupInfoVerifications",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInfoVerifications_GroupID",
                table: "GroupInfoVerifications",
                column: "GroupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupInfoVerifications");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddOrderValidationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderAvailables",
                columns: table => new
                {
                    OrderAvailableID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    CreatedByID = table.Column<string>(nullable: true),
                    _CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAvailables", x => x.OrderAvailableID);
                    table.ForeignKey(
                        name: "FK_OrderAvailables_AspNetUsers_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAvailables_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderAvailables_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderValidations",
                columns: table => new
                {
                    OrderValidationID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    ValidatedByID = table.Column<string>(nullable: true),
                    _DateValidated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderValidations", x => x.OrderValidationID);
                    table.ForeignKey(
                        name: "FK_OrderValidations_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderValidations_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderValidations_AspNetUsers_ValidatedByID",
                        column: x => x.ValidatedByID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderAvailables_CreatedByID",
                table: "OrderAvailables",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAvailables_CustomerID",
                table: "OrderAvailables",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAvailables_OrderID",
                table: "OrderAvailables",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderValidations_CustomerID",
                table: "OrderValidations",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderValidations_OrderID",
                table: "OrderValidations",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderValidations_ValidatedByID",
                table: "OrderValidations",
                column: "ValidatedByID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderAvailables");

            migrationBuilder.DropTable(
                name: "OrderValidations");
        }
    }
}

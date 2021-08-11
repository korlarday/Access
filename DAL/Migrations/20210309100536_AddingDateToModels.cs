using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allprimetech.DAL.Migrations
{
    public partial class AddingDateToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "_CreationDate",
                table: "GroupsInfos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "_UpdatedDate",
                table: "GroupsInfos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "_CreationDate",
                table: "Discs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "_UpdatedDate",
                table: "Discs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "_UpdatedDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_CreationDate",
                table: "GroupsInfos");

            migrationBuilder.DropColumn(
                name: "_UpdatedDate",
                table: "GroupsInfos");

            migrationBuilder.DropColumn(
                name: "_CreationDate",
                table: "Discs");

            migrationBuilder.DropColumn(
                name: "_UpdatedDate",
                table: "Discs");

            migrationBuilder.DropColumn(
                name: "_UpdatedDate",
                table: "AspNetUsers");
        }
    }
}

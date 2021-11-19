using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Application.Infrastructure.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Katanas",
                newName: "Title");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Prices",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Katanas",
                newName: "Name");
        }
    }
}

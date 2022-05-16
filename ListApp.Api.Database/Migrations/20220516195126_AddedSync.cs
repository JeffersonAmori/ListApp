using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListApp.Api.Database.Migrations
{
    public partial class AddedSync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Lists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChangedDate",
                table: "Lists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "Lists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "ListItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ListItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChangedDate",
                table: "ListItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Lists");

            migrationBuilder.DropColumn(
                name: "LastChangedDate",
                table: "Lists");

            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "Lists");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "ListItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ListItems");

            migrationBuilder.DropColumn(
                name: "LastChangedDate",
                table: "ListItems");
        }
    }
}

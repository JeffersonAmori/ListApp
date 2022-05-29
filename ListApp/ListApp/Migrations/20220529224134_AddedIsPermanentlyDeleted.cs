using Microsoft.EntityFrameworkCore.Migrations;

namespace ListApp.Migrations
{
    public partial class AddedIsPermanentlyDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPermanentlyDeleted",
                table: "Lists",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPermanentlyDeleted",
                table: "Lists");
        }
    }
}

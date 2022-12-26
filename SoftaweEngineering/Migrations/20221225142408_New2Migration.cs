using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftaweEngineering.Migrations
{
    public partial class New2Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Page",
                table: "Library_Book",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Page",
                table: "Library_Book");
        }
    }
}

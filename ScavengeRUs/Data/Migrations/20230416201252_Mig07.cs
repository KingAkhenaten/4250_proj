using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengeRUs.Data.Migrations
{
    public partial class Mig07 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "Hunts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URL",
                table: "Hunts");
        }
    }
}

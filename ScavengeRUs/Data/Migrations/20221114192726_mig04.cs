using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengeRUs.Data.Migrations
{
    public partial class mig04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hunts_Location_LocationId",
                table: "Hunts");

            migrationBuilder.DropIndex(
                name: "IX_Hunts_LocationId",
                table: "Hunts");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Hunts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Hunts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hunts_LocationId",
                table: "Hunts",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hunts_Location_LocationId",
                table: "Hunts",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }
    }
}

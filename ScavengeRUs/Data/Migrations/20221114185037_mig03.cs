using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengeRUs.Data.Migrations
{
    public partial class mig03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Hunts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HuntLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HuntId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuntLocation_Hunts_HuntId",
                        column: x => x.HuntId,
                        principalTable: "Hunts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HuntLocation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hunts_LocationId",
                table: "Hunts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HuntLocation_HuntId",
                table: "HuntLocation",
                column: "HuntId");

            migrationBuilder.CreateIndex(
                name: "IX_HuntLocation_LocationId",
                table: "HuntLocation",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hunts_Location_LocationId",
                table: "Hunts",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hunts_Location_LocationId",
                table: "Hunts");

            migrationBuilder.DropTable(
                name: "HuntLocation");

            migrationBuilder.DropIndex(
                name: "IX_Hunts_LocationId",
                table: "Hunts");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Hunts");
        }
    }
}

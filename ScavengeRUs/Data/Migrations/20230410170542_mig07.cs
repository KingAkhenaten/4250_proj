using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengeRUs.Data.Migrations
{
    public partial class mig07 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_AspNetUsers_ApplicationUserId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_ApplicationUserId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Location");

            migrationBuilder.CreateTable(
                name: "LocationUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    locationId = table.Column<int>(type: "INTEGER", nullable: false),
                    userId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationUser_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationUser_Location_locationId",
                        column: x => x.locationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationUser_locationId",
                table: "LocationUser",
                column: "locationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationUser_userId",
                table: "LocationUser",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationUser");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Location",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_ApplicationUserId",
                table: "Location",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_AspNetUsers_ApplicationUserId",
                table: "Location",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

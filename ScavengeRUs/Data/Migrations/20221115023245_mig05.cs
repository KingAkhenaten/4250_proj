using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengeRUs.Data.Migrations
{
    public partial class mig05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HuntId",
                table: "Location");

            migrationBuilder.AlterColumn<double>(
                name: "Lon",
                table: "Location",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "Location",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<double>(
                name: "Lon",
                table: "Location",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "Location",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "HuntId",
                table: "Location",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}

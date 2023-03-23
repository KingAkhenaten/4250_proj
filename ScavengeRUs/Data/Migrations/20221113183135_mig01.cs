using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengeRUs.Data.Migrations
{
    public partial class mig01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessCodes_Hunts_HuntId",
                table: "AccessCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AccessCodes_AccessCodeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hunts_HuntId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "HuntId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessCodes_Hunts_HuntId",
                table: "AccessCodes",
                column: "HuntId",
                principalTable: "Hunts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AccessCodes_AccessCodeId",
                table: "AspNetUsers",
                column: "AccessCodeId",
                principalTable: "AccessCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hunts_HuntId",
                table: "AspNetUsers",
                column: "HuntId",
                principalTable: "Hunts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessCodes_Hunts_HuntId",
                table: "AccessCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AccessCodes_AccessCodeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hunts_HuntId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "HuntId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessCodes_Hunts_HuntId",
                table: "AccessCodes",
                column: "HuntId",
                principalTable: "Hunts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AccessCodes_AccessCodeId",
                table: "AspNetUsers",
                column: "AccessCodeId",
                principalTable: "AccessCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hunts_HuntId",
                table: "AspNetUsers",
                column: "HuntId",
                principalTable: "Hunts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

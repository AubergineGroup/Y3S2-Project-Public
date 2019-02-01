using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sem2WebApp.Migrations
{
    public partial class UpdateToiletLogModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "ToiletLogs",
                type: "smalldatetime",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "ToiletLogs",
                type: "smalldatetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "ToiletLogs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "ToiletLogs",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "smalldatetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "ToiletLogs",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "smalldatetime");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "ToiletLogs",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}

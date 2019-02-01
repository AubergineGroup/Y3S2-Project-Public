using Microsoft.EntityFrameworkCore.Migrations;

namespace Sem2WebApp.Migrations
{
    public partial class AddUserThresholdDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserThreshold",
                table: "ToiletsSettings",
                nullable: false,
                defaultValue: 20,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserThreshold",
                table: "ToiletsSettings",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 20);
        }
    }
}

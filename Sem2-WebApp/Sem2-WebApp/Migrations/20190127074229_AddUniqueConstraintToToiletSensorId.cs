using Microsoft.EntityFrameworkCore.Migrations;

namespace Sem2WebApp.Migrations
{
    public partial class AddUniqueConstraintToToiletSensorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SensorId",
                table: "Toilets",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Toilets_SensorId",
                table: "Toilets",
                column: "SensorId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Toilets_SensorId",
                table: "Toilets");

            migrationBuilder.AlterColumn<string>(
                name: "SensorId",
                table: "Toilets",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Sem2WebApp.Migrations
{
    public partial class AddToiletGenderDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ToiletGender",
                table: "Toilets",
                maxLength: 6,
                nullable: false,
                defaultValue: "Female",
                oldClrType: typeof(string),
                oldMaxLength: 6);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ToiletGender",
                table: "Toilets",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 6,
                oldDefaultValue: "Female");
        }
    }
}

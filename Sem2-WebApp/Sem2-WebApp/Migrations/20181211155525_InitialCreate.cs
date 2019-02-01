using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sem2WebApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cleaners",
                columns: table => new
                {
                    CleanerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CleanerOwner = table.Column<string>(maxLength: 36, nullable: false),
                    FirstName = table.Column<string>(maxLength: 64, nullable: false),
                    LastName = table.Column<string>(maxLength: 64, nullable: false),
                    Email = table.Column<string>(maxLength: 254, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 32, nullable: false),
                    Rfid = table.Column<string>(maxLength: 18, nullable: false, defaultValue: "No card registered"),
                    Status = table.Column<string>(maxLength: 20, nullable: false, defaultValue: "Available"),
                    CleanedCount = table.Column<int>(nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cleaners", x => x.CleanerId);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Rating = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                });

            migrationBuilder.CreateTable(
                name: "Toilets",
                columns: table => new
                {
                    ToiletId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToiletOwner = table.Column<string>(maxLength: 36, nullable: false),
                    ToiletName = table.Column<string>(maxLength: 64, nullable: false),
                    ToiletGender = table.Column<string>(maxLength: 6, nullable: false),
                    LastCleaned = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    Comments = table.Column<string>(maxLength: 100, nullable: true),
                    SensorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toilets", x => x.ToiletId);
                });

            migrationBuilder.CreateTable(
                name: "SensorsValues",
                columns: table => new
                {
                    SensorValuesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToiletId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    CurrentUsers = table.Column<int>(nullable: false),
                    TotalUsers = table.Column<int>(nullable: false),
                    GasValue = table.Column<int>(nullable: false),
                    Requests = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsValues", x => x.SensorValuesId);
                    table.ForeignKey(
                        name: "FK_SensorsValues_Toilets_ToiletId",
                        column: x => x.ToiletId,
                        principalTable: "Toilets",
                        principalColumn: "ToiletId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToiletLogs",
                columns: table => new
                {
                    ToiletLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToiletId = table.Column<int>(nullable: false),
                    CleanerId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Duration = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToiletLogs", x => x.ToiletLogId);
                    table.ForeignKey(
                        name: "FK_ToiletLogs_Cleaners_CleanerId",
                        column: x => x.CleanerId,
                        principalTable: "Cleaners",
                        principalColumn: "CleanerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToiletLogs_Toilets_ToiletId",
                        column: x => x.ToiletId,
                        principalTable: "Toilets",
                        principalColumn: "ToiletId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToiletsSettings",
                columns: table => new
                {
                    ToiletSettingsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToiletId = table.Column<int>(nullable: false),
                    UpdateFrequency = table.Column<int>(nullable: false, defaultValue: 1000),
                    FanMode = table.Column<string>(maxLength: 4, nullable: false, defaultValue: "Auto"),
                    FanThreshold = table.Column<int>(nullable: false, defaultValue: 40),
                    UserThreshold = table.Column<int>(nullable: false),
                    GasValueThreshold = table.Column<int>(nullable: false, defaultValue: 60),
                    RequestThreshold = table.Column<int>(nullable: false, defaultValue: 10)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToiletsSettings", x => x.ToiletSettingsId);
                    table.ForeignKey(
                        name: "FK_ToiletsSettings_Toilets_ToiletId",
                        column: x => x.ToiletId,
                        principalTable: "Toilets",
                        principalColumn: "ToiletId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorsValues_ToiletId",
                table: "SensorsValues",
                column: "ToiletId");

            migrationBuilder.CreateIndex(
                name: "IX_ToiletLogs_CleanerId",
                table: "ToiletLogs",
                column: "CleanerId");

            migrationBuilder.CreateIndex(
                name: "IX_ToiletLogs_ToiletId",
                table: "ToiletLogs",
                column: "ToiletId");

            migrationBuilder.CreateIndex(
                name: "IX_ToiletsSettings_ToiletId",
                table: "ToiletsSettings",
                column: "ToiletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SensorsValues");

            migrationBuilder.DropTable(
                name: "ToiletLogs");

            migrationBuilder.DropTable(
                name: "ToiletsSettings");

            migrationBuilder.DropTable(
                name: "Cleaners");

            migrationBuilder.DropTable(
                name: "Toilets");
        }
    }
}

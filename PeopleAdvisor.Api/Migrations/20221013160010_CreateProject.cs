using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeopleAdvisor.Api.Migrations
{
    public partial class CreateProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "Date", nullable: true),
                    Currency = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CurrentSalary = table.Column<double>(type: "double precision", nullable: true),
                    Degree = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    ExperienceYears = table.Column<int>(type: "integer", nullable: true),
                    FirstName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Gender = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Languages = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Linkedin = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    NoticePeriod = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ParsedResume = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    RemoteOnly = table.Column<bool>(type: "boolean", nullable: true),
                    Role = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    School = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    Sector = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    Skills = table.Column<string>(type: "text", nullable: true),
                    StudyLevel = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "Date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Experience",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Company = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EndDate = table.Column<DateTime>(type: "Date", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: true),
                    StartDate = table.Column<DateTime>(type: "Date", nullable: true),
                    Title = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experience_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Experience_ProfileId",
                table: "Experience",
                column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Experience");

            migrationBuilder.DropTable(
                name: "Profile");
        }
    }
}

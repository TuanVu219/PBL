using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3Hos.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "qualification",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "studyprocess",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "yearofexperience",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "qualification",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "studyprocess",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "yearofexperience",
                table: "Doctors");
        }
    }
}

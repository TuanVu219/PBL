using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3Hos.Migrations
{
    /// <inheritdoc />
    public partial class ImgFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgFile",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgFile",
                table: "Patients");
        }
    }
}

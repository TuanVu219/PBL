using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3Hos.Migrations
{
    /// <inheritdoc />
    public partial class DoctorAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
              name: "DoctorAvailabilities",
              columns: table => new
              {
                  Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  AvailableDate = table.Column<DateOnly>(type: "date", nullable: false),
                  StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                  EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                  DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_DoctorAvailabilities", x => x.Id);
                  table.ForeignKey(
                      name: "FK_DoctorAvailabilities_Doctors_DoctorId",
                      column: x => x.DoctorId,
                      principalTable: "Doctors",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
              });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilities_DoctorId",
                table: "DoctorAvailabilities",
                column: "DoctorId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
              name: "DoctorAvailabilities");
        }
    }
}

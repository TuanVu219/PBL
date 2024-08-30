using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3Hos.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistories_Appointments_AppointmentId",
                table: "AppointmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentHistories_AppointmentId",
                table: "AppointmentHistories");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "AppointmentHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "AppointmentHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentHistories_AppointmentId",
                table: "AppointmentHistories",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistories_Appointments_AppointmentId",
                table: "AppointmentHistories",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

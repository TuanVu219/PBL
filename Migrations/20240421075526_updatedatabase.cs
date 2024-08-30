using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3Hos.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Doctorid",
                table: "AppointmentHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patientid",
                table: "AppointmentHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentHistories_Doctorid",
                table: "AppointmentHistories",
                column: "Doctorid");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentHistories_Patientid",
                table: "AppointmentHistories",
                column: "Patientid");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistories_Doctors_Doctorid",
                table: "AppointmentHistories",
                column: "Doctorid",
                principalTable: "Doctors",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistories_Patients_Patientid",
                table: "AppointmentHistories",
                column: "Patientid",
                principalTable: "Patients",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistories_Doctors_Doctorid",
                table: "AppointmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistories_Patients_Patientid",
                table: "AppointmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentHistories_Doctorid",
                table: "AppointmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentHistories_Patientid",
                table: "AppointmentHistories");

            migrationBuilder.DropColumn(
                name: "Doctorid",
                table: "AppointmentHistories");

            migrationBuilder.DropColumn(
                name: "Patientid",
                table: "AppointmentHistories");
        }
    }
}

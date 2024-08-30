using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3Hos.Migrations
{
    /// <inheritdoc />
    public partial class Delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Doctorid = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Patientid = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoctorAppointmentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientAppointmentId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentHistories_Doctors_Doctorid",
                        column: x => x.Doctorid,
                        principalTable: "Doctors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AppointmentHistories_Patients_Patientid",
                        column: x => x.Patientid,
                        principalTable: "Patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentHistories_Doctorid",
                table: "AppointmentHistories",
                column: "Doctorid");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentHistories_Patientid",
                table: "AppointmentHistories",
                column: "Patientid");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_ClientsPackagesAppointments_ClientPackageAppointmentId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ClientPackageAppointmentId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientPackageAppointmentId",
                table: "Appointments");

            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                table: "ClientsPackagesAppointments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientsPackagesAppointments_AppointmentId",
                table: "ClientsPackagesAppointments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsPackagesAppointments_ClientId",
                table: "ClientsPackagesAppointments",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsPackagesAppointments_Appointments_AppointmentId",
                table: "ClientsPackagesAppointments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsPackagesAppointments_Clients_ClientId",
                table: "ClientsPackagesAppointments",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientsPackagesAppointments_Appointments_AppointmentId",
                table: "ClientsPackagesAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientsPackagesAppointments_Clients_ClientId",
                table: "ClientsPackagesAppointments");

            migrationBuilder.DropIndex(
                name: "IX_ClientsPackagesAppointments_AppointmentId",
                table: "ClientsPackagesAppointments");

            migrationBuilder.DropIndex(
                name: "IX_ClientsPackagesAppointments_ClientId",
                table: "ClientsPackagesAppointments");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClientsPackagesAppointments");

            migrationBuilder.AddColumn<long>(
                name: "ClientPackageAppointmentId",
                table: "Appointments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientPackageAppointmentId",
                table: "Appointments",
                column: "ClientPackageAppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_ClientsPackagesAppointments_ClientPackageAppointmentId",
                table: "Appointments",
                column: "ClientPackageAppointmentId",
                principalTable: "ClientsPackagesAppointments",
                principalColumn: "Id");
        }
    }
}

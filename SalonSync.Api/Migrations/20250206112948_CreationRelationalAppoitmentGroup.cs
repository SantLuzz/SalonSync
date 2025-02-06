using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreationRelationalAppoitmentGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AppointmentGroupId",
                table: "Appointments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppointmentGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentGroupId",
                table: "Appointments",
                column: "AppointmentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentGroups_AppointmentGroupId",
                table: "Appointments",
                column: "AppointmentGroupId",
                principalTable: "AppointmentGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentGroups_AppointmentGroupId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentGroups");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentGroupId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentGroupId",
                table: "Appointments");
        }
    }
}

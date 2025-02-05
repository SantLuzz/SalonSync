using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreationAppointmentstables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Employees",
                type: "TIME",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Employees",
                type: "TIME",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Price = table.Column<decimal>(type: "MONEY", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Duration = table.Column<int>(type: "INT", nullable: false, defaultValue: 30),
                    Price = table.Column<decimal>(type: "MONEY", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientsPackages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    StartDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    RemainingSessions = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                    ClientId = table.Column<long>(type: "BIGINT", nullable: false),
                    PackageId = table.Column<long>(type: "BIGINT", nullable: false),
                    Status = table.Column<short>(type: "SMALLINT", nullable: false, defaultValue: (short)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientsPackages_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientsPackages_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageServices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    PackageId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    SessionsPerService = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageServices_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientsPackagesAppointments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    ClientPackageId = table.Column<long>(type: "bigint", nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsPackagesAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientsPackagesAppointments_ClientsPackages_ClientPackageId",
                        column: x => x.ClientPackageId,
                        principalTable: "ClientsPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Date = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "TIME", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "TIME", nullable: false),
                    Status = table.Column<short>(type: "SMALLINT", nullable: false, defaultValue: (short)1),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ClientPackageAppointmentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_ClientsPackagesAppointments_ClientPackageAppointmentId",
                        column: x => x.ClientPackageAppointmentId,
                        principalTable: "ClientsPackagesAppointments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientId",
                table: "Appointments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientPackageAppointmentId",
                table: "Appointments",
                column: "ClientPackageAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EmployeeId",
                table: "Appointments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceId",
                table: "Appointments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsPackages_ClientId",
                table: "ClientsPackages",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsPackages_PackageId",
                table: "ClientsPackages",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsPackagesAppointments_ClientPackageId",
                table: "ClientsPackagesAppointments",
                column: "ClientPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageServices_PackageId",
                table: "PackageServices",
                column: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "PackageServices");

            migrationBuilder.DropTable(
                name: "ClientsPackagesAppointments");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "ClientsPackages");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Employees");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreationAppointmentsCanceleationHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentsCancellationsHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "TIME", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "TIME", nullable: false),
                    Status = table.Column<short>(type: "SMALLINT", nullable: false, defaultValue: (short)3),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: true),
                    AppointmentGroupId = table.Column<long>(type: "bigint", nullable: true),
                    CancellationReason = table.Column<string>(type: "NVARCHAR(max)", maxLength: -1, nullable: true),
                    CanceledBy = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    CanceledAt = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentsCancellationsHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                table: "Companies",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentsCancellationsHistories");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserId",
                table: "Companies");
        }
    }
}

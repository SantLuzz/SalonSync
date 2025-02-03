using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: true),
                    PreferredService = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    Observations = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    Type = table.Column<short>(type: "SMALLINT", nullable: false, defaultValue: (short)1),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastProcedure = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: true),
                    Position = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()"),
                    Admission = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    Commission = table.Column<decimal>(type: "MONEY", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class createAccountPayable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountsPayable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    SupplierName = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: false),
                    SupplierDocument = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<decimal>(type: "MONEY", nullable: false),
                    DueDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsPayable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountsPayable");
        }
    }
}

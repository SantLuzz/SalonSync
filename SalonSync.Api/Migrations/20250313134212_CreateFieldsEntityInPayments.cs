using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateFieldsEntityInPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EntityId",
                table: "Transactions",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "EntityType",
                table: "Transactions",
                type: "SMALLINT",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_PackageServices_ServiceId",
                table: "PackageServices",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageServices_Services_ServiceId",
                table: "PackageServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageServices_Services_ServiceId",
                table: "PackageServices");

            migrationBuilder.DropIndex(
                name: "IX_PackageServices_ServiceId",
                table: "PackageServices");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Transactions");
        }
    }
}

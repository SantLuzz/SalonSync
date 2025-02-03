using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonSync.Api.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesCamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Employees",
                type: "NVARCHAR(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Clients",
                type: "NVARCHAR(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Employees",
                type: "nvarchar(max)",
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}

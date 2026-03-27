using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notas.Nzx.Migrations
{
    /// <inheritdoc />
    public partial class AgregandoPlanYApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Plan",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestsHoy",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoReset",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Plan",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RequestsHoy",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UltimoReset",
                table: "Usuarios");
        }
    }
}

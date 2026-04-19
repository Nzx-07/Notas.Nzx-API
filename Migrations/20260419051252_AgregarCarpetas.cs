using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notas.Nzx.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCarpetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CarpetaId",
                table: "Notas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Carpetas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carpetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carpetas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notas_CarpetaId",
                table: "Notas",
                column: "CarpetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Carpetas_UsuarioId",
                table: "Carpetas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notas_Carpetas_CarpetaId",
                table: "Notas",
                column: "CarpetaId",
                principalTable: "Carpetas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notas_Carpetas_CarpetaId",
                table: "Notas");

            migrationBuilder.DropTable(
                name: "Carpetas");

            migrationBuilder.DropIndex(
                name: "IX_Notas_CarpetaId",
                table: "Notas");

            migrationBuilder.DropColumn(
                name: "CarpetaId",
                table: "Notas");
        }
    }
}

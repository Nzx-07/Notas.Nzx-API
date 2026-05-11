using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notas.Nzx.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTemaActivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemaActivo",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemaActivo",
                table: "Usuarios");
        }
    }
}

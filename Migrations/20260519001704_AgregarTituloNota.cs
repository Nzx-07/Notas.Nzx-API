using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notas.Nzx.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTituloNota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Notas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Notas");
        }
    }
}

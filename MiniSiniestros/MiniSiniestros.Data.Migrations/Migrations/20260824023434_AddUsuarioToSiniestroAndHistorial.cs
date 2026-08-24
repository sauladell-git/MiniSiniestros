using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSiniestros.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioToSiniestroAndHistorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Siniestros",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "SiniestroEstadoHistoriales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Siniestros_UsuarioId",
                table: "Siniestros",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SiniestroEstadoHistoriales_UsuarioId",
                table: "SiniestroEstadoHistoriales",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_SiniestroEstadoHistoriales_Usuarios_UsuarioId",
                table: "SiniestroEstadoHistoriales",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Siniestros_Usuarios_UsuarioId",
                table: "Siniestros",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiniestroEstadoHistoriales_Usuarios_UsuarioId",
                table: "SiniestroEstadoHistoriales");

            migrationBuilder.DropForeignKey(
                name: "FK_Siniestros_Usuarios_UsuarioId",
                table: "Siniestros");

            migrationBuilder.DropIndex(
                name: "IX_Siniestros_UsuarioId",
                table: "Siniestros");

            migrationBuilder.DropIndex(
                name: "IX_SiniestroEstadoHistoriales_UsuarioId",
                table: "SiniestroEstadoHistoriales");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Siniestros");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "SiniestroEstadoHistoriales");
        }
    }
}

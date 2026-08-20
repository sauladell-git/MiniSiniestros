using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSiniestros.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empleadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prestadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiniestroEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiniestroEstados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trabajadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmpleadorId = table.Column<int>(type: "int", nullable: false),
                    Cuil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trabajadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trabajadores_Empleadores_EmpleadorId",
                        column: x => x.EmpleadorId,
                        principalTable: "Empleadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Siniestros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmpleadorId = table.Column<int>(type: "int", nullable: false),
                    TrabajadorId = table.Column<int>(type: "int", nullable: false),
                    SiniestroEstadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siniestros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siniestros_Empleadores_EmpleadorId",
                        column: x => x.EmpleadorId,
                        principalTable: "Empleadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Siniestros_SiniestroEstados_SiniestroEstadoId",
                        column: x => x.SiniestroEstadoId,
                        principalTable: "SiniestroEstados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Siniestros_Trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "Trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiniestroEstadoHistoriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SiniestroId = table.Column<int>(type: "int", nullable: false),
                    SiniestroEstadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiniestroEstadoHistoriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiniestroEstadoHistoriales_SiniestroEstados_SiniestroEstadoId",
                        column: x => x.SiniestroEstadoId,
                        principalTable: "SiniestroEstados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiniestroEstadoHistoriales_Siniestros_SiniestroId",
                        column: x => x.SiniestroId,
                        principalTable: "Siniestros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiniestroPrestadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiniestroId = table.Column<int>(type: "int", nullable: false),
                    PrestadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiniestroPrestadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiniestroPrestadores_Prestadores_PrestadorId",
                        column: x => x.PrestadorId,
                        principalTable: "Prestadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiniestroPrestadores_Siniestros_SiniestroId",
                        column: x => x.SiniestroId,
                        principalTable: "Siniestros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleadores_Cuit",
                table: "Empleadores",
                column: "Cuit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiniestroEstadoHistoriales_SiniestroEstadoId",
                table: "SiniestroEstadoHistoriales",
                column: "SiniestroEstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SiniestroEstadoHistoriales_SiniestroId",
                table: "SiniestroEstadoHistoriales",
                column: "SiniestroId");

            migrationBuilder.CreateIndex(
                name: "IX_SiniestroPrestadores_PrestadorId",
                table: "SiniestroPrestadores",
                column: "PrestadorId");

            migrationBuilder.CreateIndex(
                name: "IX_SiniestroPrestadores_SiniestroId",
                table: "SiniestroPrestadores",
                column: "SiniestroId");

            migrationBuilder.CreateIndex(
                name: "IX_Siniestros_EmpleadorId",
                table: "Siniestros",
                column: "EmpleadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Siniestros_SiniestroEstadoId",
                table: "Siniestros",
                column: "SiniestroEstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Siniestros_TrabajadorId",
                table: "Siniestros",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_EmpleadorId",
                table: "Trabajadores",
                column: "EmpleadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiniestroEstadoHistoriales");

            migrationBuilder.DropTable(
                name: "SiniestroPrestadores");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Prestadores");

            migrationBuilder.DropTable(
                name: "Siniestros");

            migrationBuilder.DropTable(
                name: "SiniestroEstados");

            migrationBuilder.DropTable(
                name: "Trabajadores");

            migrationBuilder.DropTable(
                name: "Empleadores");
        }
    }
}

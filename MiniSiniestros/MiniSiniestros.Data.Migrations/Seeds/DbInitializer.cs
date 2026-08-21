using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Common.Enums;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Migrations.Seeds
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(MiniSiniestrosDbContext context)
        {
            // 1. Eliminar base de datos previa 'MiniSiniestros' si existe para arranque limpio
            await context.Database.EnsureDeletedAsync();

            // 2. Aplicar migraciones desde cero para 'MiniSiniestros'
            await context.Database.MigrateAsync();

            // 3. Seed SiniestroEstados (Recibido, EnAnalisis, Aprobado, Rechazado, Cerrado)
            var estados = new List<SiniestroEstado>
            {
                new() { Nombre = SiniestroEstadoEnum.Recibido.ToString() },
                new() { Nombre = SiniestroEstadoEnum.EnAnalisis.ToString() },
                new() { Nombre = SiniestroEstadoEnum.Aprobado.ToString() },
                new() { Nombre = SiniestroEstadoEnum.Rechazado.ToString() },
                new() { Nombre = SiniestroEstadoEnum.Cerrado.ToString() }
            };

            await context.SiniestroEstados.AddRangeAsync(estados);
            await context.SaveChangesAsync();

            // 4. Seed Roles
            var roles = new List<Rol>
            {
                new() { Nombre = "Administrador", Descripcion = "Acceso total a la plataforma" },
                new() { Nombre = "Analista", Descripcion = "Gestión y revisión de siniestros" },
                new() { Nombre = "Operador", Descripcion = "Carga de siniestros y consultas" }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();

            // 5. Seed Usuarios (3 usuarios: Admin, Operador, Analista)
            var usuarios = new List<Usuario>
            {
                new()
                {
                    Nombre = "Admin",
                    Apellido = "Sistema",
                    Password = "Admin*2026"
                },
                new()
                {
                    Nombre = "Operador",
                    Apellido = "Siniestros",
                    Password = "Operador*2026"
                },
                new()
                {
                    Nombre = "Analista",
                    Apellido = "Revisiones",
                    Password = "Analista*2026"
                }
            };

            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();

            // 6. Seed Usuario_Rol
            var rolAdmin = await context.Roles.FirstAsync(r => r.Nombre == "Administrador");
            var rolOperador = await context.Roles.FirstAsync(r => r.Nombre == "Operador");
            var rolAnalista = await context.Roles.FirstAsync(r => r.Nombre == "Analista");

            var usuarioAdmin = await context.Usuarios.FirstAsync(u => u.Nombre == "Admin");
            var usuarioOperador = await context.Usuarios.FirstAsync(u => u.Nombre == "Operador");
            var usuarioAnalista = await context.Usuarios.FirstAsync(u => u.Nombre == "Analista");

            var usuarioRolesSeed = new List<Usuario_Rol>
            {
                new() { UsuarioId = usuarioAdmin.Id, RolId = rolAdmin.Id },
                new() { UsuarioId = usuarioOperador.Id, RolId = rolOperador.Id },
                new() { UsuarioId = usuarioAnalista.Id, RolId = rolAnalista.Id }
            };

            await context.UsuarioRoles.AddRangeAsync(usuarioRolesSeed);
            await context.SaveChangesAsync();

            // 5. Seed Empleadores (Empresas A, B, C, D, E, F)
            var empleadores = new List<Empleador>
            {
                new()
                {
                    RazonSocial = "Empresa A S.A.",
                    Cuit = "30111111111"
                },
                new()
                {
                    RazonSocial = "Empresa B S.A.",
                    Cuit = "30222222222"
                },
                new()
                {
                    RazonSocial = "Empresa C S.A.",
                    Cuit = "30333333333"
                },
                new()
                {
                    RazonSocial = "Empresa D S.A.",
                    Cuit = "30444444444"
                },
                new()
                {
                    RazonSocial = "Empresa E S.A.",
                    Cuit = "30555555555"
                },
                new()
                {
                    RazonSocial = "Empresa F S.A.",
                    Cuit = "30666666666"
                }
            };

            await context.Empleadores.AddRangeAsync(empleadores);
            await context.SaveChangesAsync();

            // 6. Seed Prestadores
            var prestadores = new List<Prestador>
            {
                new() { Nombre = "Sanatorio Colegiales" },
                new() { Nombre = "Sanatorio Santa Isabel" },
                new() { Nombre = "Hospital Británico" },
                new() { Nombre = "IADT" },
                new() { Nombre = "Hospital Sirio Libanés" }
            };

            await context.Prestadores.AddRangeAsync(prestadores);
            await context.SaveChangesAsync();

            // 7. Seed Trabajadores
            var empresaA = await context.Empleadores.FirstAsync(e => e.Cuit == "30111111111");
            var empresaB = await context.Empleadores.FirstAsync(e => e.Cuit == "30222222222");
            var empresaC = await context.Empleadores.FirstAsync(e => e.Cuit == "30333333333");
            var empresaD = await context.Empleadores.FirstAsync(e => e.Cuit == "30444444444");
            var empresaE = await context.Empleadores.FirstAsync(e => e.Cuit == "30555555555");

            var trabajadores = new List<Trabajador>
            {
                new()
                {
                    Nombre = "Charly",
                    Apellido = "García",
                    Cuil = "20111111111",
                    EmpleadorId = empresaA.Id
                },
                new()
                {
                    Nombre = "Gustavo",
                    Apellido = "Cerati",
                    Cuil = "20222222222",
                    EmpleadorId = empresaB.Id
                },
                new()
                {
                    Nombre = "Carlos",
                    Apellido = "Solari",
                    Cuil = "20333333333",
                    EmpleadorId = empresaC.Id
                },
                new()
                {
                    Nombre = "Astor",
                    Apellido = "Piazzolla",
                    Cuil = "20444444444",
                    EmpleadorId = empresaD.Id
                },
                new()
                {
                    Nombre = "Luis Alberto",
                    Apellido = "Spinetta",
                    Cuil = "20555555555",
                    EmpleadorId = empresaE.Id
                }
            };

            await context.Trabajadores.AddRangeAsync(trabajadores);
            await context.SaveChangesAsync();

            // 8. Seed Siniestros
            var estadoRecibidoStr = SiniestroEstadoEnum.Recibido.ToString();
            var estadoEnAnalisisStr = SiniestroEstadoEnum.EnAnalisis.ToString();
            var estadoAprobadoStr = SiniestroEstadoEnum.Aprobado.ToString();
            var estadoRechazadoStr = SiniestroEstadoEnum.Rechazado.ToString();
            var estadoCerradoStr = SiniestroEstadoEnum.Cerrado.ToString();

            var estadoRecibido = await context.SiniestroEstados.FirstAsync(e => e.Nombre == estadoRecibidoStr);
            var estadoEnAnalisis = await context.SiniestroEstados.FirstAsync(e => e.Nombre == estadoEnAnalisisStr);
            var estadoAprobado = await context.SiniestroEstados.FirstAsync(e => e.Nombre == estadoAprobadoStr);
            var estadoRechazado = await context.SiniestroEstados.FirstAsync(e => e.Nombre == estadoRechazadoStr);
            var estadoCerrado = await context.SiniestroEstados.FirstAsync(e => e.Nombre == estadoCerradoStr);

            var trabajadorCharly = await context.Trabajadores.FirstAsync(t => t.Cuil == "20111111111");
            var trabajadorCerati = await context.Trabajadores.FirstAsync(t => t.Cuil == "20222222222");
            var trabajadorSolari = await context.Trabajadores.FirstAsync(t => t.Cuil == "20333333333");
            var trabajadorAstor = await context.Trabajadores.FirstAsync(t => t.Cuil == "20444444444");
            var trabajadorSpinetta = await context.Trabajadores.FirstAsync(t => t.Cuil == "20555555555");

            var siniestros = new List<Siniestro>
            {
                new()
                {
                    Numero = 1001,
                    Fecha = DateTime.UtcNow.AddDays(-15),
                    Observaciones = "Incidente reportado en planta industrial durante jornada laboral.",
                    EmpleadorId = empresaA.Id,
                    TrabajadorId = trabajadorCharly.Id,
                    SiniestroEstadoId = estadoRecibido.Id
                },
                new()
                {
                    Numero = 1002,
                    Fecha = DateTime.UtcNow.AddDays(-10),
                    Observaciones = "Revisión médica y peritaje en proceso por caída en oficina.",
                    EmpleadorId = empresaB.Id,
                    TrabajadorId = trabajadorCerati.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id
                },
                new()
                {
                    Numero = 1003,
                    Fecha = DateTime.UtcNow.AddDays(-30),
                    Observaciones = "Tratamiento finalizado, alta médica otorgada y siniestro aprobado.",
                    EmpleadorId = empresaC.Id,
                    TrabajadorId = trabajadorSolari.Id,
                    SiniestroEstadoId = estadoAprobado.Id
                },
                new()
                {
                    Numero = 1004,
                    Fecha = DateTime.UtcNow.AddDays(-60),
                    Observaciones = "Rechazado debido a inconsistencias en la documentación respaldatoria.",
                    EmpleadorId = empresaD.Id,
                    TrabajadorId = trabajadorAstor.Id,
                    SiniestroEstadoId = estadoRechazado.Id
                },
                new()
                {
                    Numero = 1005,
                    Fecha = DateTime.UtcNow.AddDays(-90),
                    Observaciones = "Expediente cerrado tras cumplimiento de todas las prestaciones acordadas.",
                    EmpleadorId = empresaE.Id,
                    TrabajadorId = trabajadorSpinetta.Id,
                    SiniestroEstadoId = estadoCerrado.Id
                }
            };

            await context.Siniestros.AddRangeAsync(siniestros);
            await context.SaveChangesAsync();

            // 9. Seed Siniestro_Prestador
            var siniestro1001 = await context.Siniestros.FirstAsync(s => s.Numero == 1001);
            var siniestro1002 = await context.Siniestros.FirstAsync(s => s.Numero == 1002);
            var siniestro1003 = await context.Siniestros.FirstAsync(s => s.Numero == 1003);
            var siniestro1004 = await context.Siniestros.FirstAsync(s => s.Numero == 1004);
            var siniestro1005 = await context.Siniestros.FirstAsync(s => s.Numero == 1005);

            var prestadorColegiales = await context.Prestadores.FirstAsync(p => p.Nombre.Contains("Colegiales"));
            var prestadorSantaIsabel = await context.Prestadores.FirstAsync(p => p.Nombre.Contains("Santa Isabel"));
            var prestadorBritanico = await context.Prestadores.FirstAsync(p => p.Nombre.Contains("Británico"));
            var prestadorIadt = await context.Prestadores.FirstAsync(p => p.Nombre.Contains("IADT"));
            var prestadorSirio = await context.Prestadores.FirstAsync(p => p.Nombre.Contains("Sirio Libanés"));

            var asignaciones = new List<Siniestro_Prestador>
            {
                new() { SiniestroId = siniestro1001.Id, PrestadorId = prestadorColegiales.Id },
                new() { SiniestroId = siniestro1002.Id, PrestadorId = prestadorSantaIsabel.Id },
                new() { SiniestroId = siniestro1003.Id, PrestadorId = prestadorBritanico.Id },
                new() { SiniestroId = siniestro1004.Id, PrestadorId = prestadorIadt.Id },
                new() { SiniestroId = siniestro1005.Id, PrestadorId = prestadorSirio.Id }
            };

            await context.SiniestroPrestadores.AddRangeAsync(asignaciones);
            await context.SaveChangesAsync();

            // 10. Seed SiniestroEstadoHistorial
            var historiales = new List<SiniestroEstadoHistorial>
            {
                // Historial Siniestro 1001
                new()
                {
                    SiniestroId = siniestro1001.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    Fecha = siniestro1001.Fecha
                },

                // Historial Siniestro 1002
                new()
                {
                    SiniestroId = siniestro1002.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    Fecha = siniestro1002.Fecha.AddDays(-2)
                },
                new()
                {
                    SiniestroId = siniestro1002.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    Fecha = siniestro1002.Fecha
                },

                // Historial Siniestro 1003
                new()
                {
                    SiniestroId = siniestro1003.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    Fecha = siniestro1003.Fecha.AddDays(-10)
                },
                new()
                {
                    SiniestroId = siniestro1003.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    Fecha = siniestro1003.Fecha.AddDays(-5)
                },
                new()
                {
                    SiniestroId = siniestro1003.Id,
                    SiniestroEstadoId = estadoAprobado.Id,
                    Fecha = siniestro1003.Fecha
                },

                // Historial Siniestro 1004
                new()
                {
                    SiniestroId = siniestro1004.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    Fecha = siniestro1004.Fecha.AddDays(-10)
                },
                new()
                {
                    SiniestroId = siniestro1004.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    Fecha = siniestro1004.Fecha.AddDays(-5)
                },
                new()
                {
                    SiniestroId = siniestro1004.Id,
                    SiniestroEstadoId = estadoRechazado.Id,
                    Fecha = siniestro1004.Fecha
                },

                // Historial Siniestro 1005
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    Fecha = siniestro1005.Fecha.AddDays(-30)
                },
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    Fecha = siniestro1005.Fecha.AddDays(-20)
                },
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoAprobado.Id,
                    Fecha = siniestro1005.Fecha.AddDays(-10)
                },
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoCerrado.Id,
                    Fecha = siniestro1005.Fecha
                }
            };

            await context.SiniestroEstadoHistoriales.AddRangeAsync(historiales);
            await context.SaveChangesAsync();
        }
    }
}

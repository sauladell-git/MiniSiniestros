using Microsoft.EntityFrameworkCore;
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
                new() { Nombre = "Recibido" },
                new() { Nombre = "EnAnalisis" },
                new() { Nombre = "Aprobado" },
                new() { Nombre = "Rechazado" },
                new() { Nombre = "Cerrado" }
            };

            await context.SiniestroEstados.AddRangeAsync(estados);
            await context.SaveChangesAsync();

            // 4. Seed Usuarios (2 usuarios)
            var usuarios = new List<Usuario>
            {
                new()
                {
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Password = "AdminPassword*2026"
                },
                new()
                {
                    Nombre = "María",
                    Apellido = "Rodríguez",
                    Password = "OperadorPassword*2026"
                }
            };

            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();

            // 5. Seed Empleadores (Empresas A, B, C, D, E, F)
            var empleadores = new List<Empleador>
            {
                new()
                {
                    RazonSocial = "Empresa A S.A.",
                    Cuit = "30-11111111-1"
                },
                new()
                {
                    RazonSocial = "Empresa B S.A.",
                    Cuit = "30-22222222-2"
                },
                new()
                {
                    RazonSocial = "Empresa C S.A.",
                    Cuit = "30-33333333-3"
                },
                new()
                {
                    RazonSocial = "Empresa D S.A.",
                    Cuit = "30-44444444-4"
                },
                new()
                {
                    RazonSocial = "Empresa E S.A.",
                    Cuit = "30-55555555-5"
                },
                new()
                {
                    RazonSocial = "Empresa F S.A.",
                    Cuit = "30-66666666-6"
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
            var empresaA = await context.Empleadores.FirstAsync(e => e.Cuit == "30-11111111-1");
            var empresaB = await context.Empleadores.FirstAsync(e => e.Cuit == "30-22222222-2");
            var empresaC = await context.Empleadores.FirstAsync(e => e.Cuit == "30-33333333-3");
            var empresaD = await context.Empleadores.FirstAsync(e => e.Cuit == "30-44444444-4");
            var empresaE = await context.Empleadores.FirstAsync(e => e.Cuit == "30-55555555-5");

            var trabajadores = new List<Trabajador>
            {
                new()
                {
                    Nombre = "Charly",
                    Apellido = "García",
                    Cuil = "20-11111111-1",
                    EmpleadorId = empresaA.Id
                },
                new()
                {
                    Nombre = "Gustavo",
                    Apellido = "Cerati",
                    Cuil = "20-22222222-2",
                    EmpleadorId = empresaB.Id
                },
                new()
                {
                    Nombre = "Carlos",
                    Apellido = "Solari",
                    Cuil = "20-33333333-3",
                    EmpleadorId = empresaC.Id
                },
                new()
                {
                    Nombre = "Astor",
                    Apellido = "Piazzolla",
                    Cuil = "20-44444444-4",
                    EmpleadorId = empresaD.Id
                },
                new()
                {
                    Nombre = "Luis Alberto",
                    Apellido = "Spinetta",
                    Cuil = "20-55555555-5",
                    EmpleadorId = empresaE.Id
                }
            };

            await context.Trabajadores.AddRangeAsync(trabajadores);
            await context.SaveChangesAsync();

            // 8. Seed Siniestros
            var estadoRecibido = await context.SiniestroEstados.FirstAsync(e => e.Nombre == "Recibido");
            var estadoEnAnalisis = await context.SiniestroEstados.FirstAsync(e => e.Nombre == "EnAnalisis");
            var estadoAprobado = await context.SiniestroEstados.FirstAsync(e => e.Nombre == "Aprobado");
            var estadoRechazado = await context.SiniestroEstados.FirstAsync(e => e.Nombre == "Rechazado");
            var estadoCerrado = await context.SiniestroEstados.FirstAsync(e => e.Nombre == "Cerrado");

            var trabajadorCharly = await context.Trabajadores.FirstAsync(t => t.Cuil == "20-11111111-1");
            var trabajadorCerati = await context.Trabajadores.FirstAsync(t => t.Cuil == "20-22222222-2");
            var trabajadorSolari = await context.Trabajadores.FirstAsync(t => t.Cuil == "20-33333333-3");
            var trabajadorAstor = await context.Trabajadores.FirstAsync(t => t.Cuil == "20-44444444-4");
            var trabajadorSpinetta = await context.Trabajadores.FirstAsync(t => t.Cuil == "20-55555555-5");

            var siniestros = new List<Siniestro>
            {
                new()
                {
                    Numero = 1001,
                    Fecha = DateTime.UtcNow.AddDays(-15),
                    EmpleadorId = empresaA.Id,
                    TrabajadorId = trabajadorCharly.Id,
                    SiniestroEstadoId = estadoRecibido.Id
                },
                new()
                {
                    Numero = 1002,
                    Fecha = DateTime.UtcNow.AddDays(-10),
                    EmpleadorId = empresaB.Id,
                    TrabajadorId = trabajadorCerati.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id
                },
                new()
                {
                    Numero = 1003,
                    Fecha = DateTime.UtcNow.AddDays(-30),
                    EmpleadorId = empresaC.Id,
                    TrabajadorId = trabajadorSolari.Id,
                    SiniestroEstadoId = estadoAprobado.Id
                },
                new()
                {
                    Numero = 1004,
                    Fecha = DateTime.UtcNow.AddDays(-60),
                    EmpleadorId = empresaD.Id,
                    TrabajadorId = trabajadorAstor.Id,
                    SiniestroEstadoId = estadoRechazado.Id
                },
                new()
                {
                    Numero = 1005,
                    Fecha = DateTime.UtcNow.AddDays(-90),
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
            var usuarioJuan = await context.Usuarios.FirstAsync(u => u.Nombre == "Juan");
            var usuarioMaria = await context.Usuarios.FirstAsync(u => u.Nombre == "María");

            var historiales = new List<SiniestroEstadoHistorial>
            {
                // Historial Siniestro 1001
                new()
                {
                    SiniestroId = siniestro1001.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    UsuarioId = usuarioJuan.Id,
                    Fecha = siniestro1001.Fecha
                },

                // Historial Siniestro 1002
                new()
                {
                    SiniestroId = siniestro1002.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    UsuarioId = usuarioJuan.Id,
                    Fecha = siniestro1002.Fecha.AddDays(-2)
                },
                new()
                {
                    SiniestroId = siniestro1002.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    UsuarioId = usuarioMaria.Id,
                    Fecha = siniestro1002.Fecha
                },

                // Historial Siniestro 1003
                new()
                {
                    SiniestroId = siniestro1003.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    UsuarioId = usuarioJuan.Id,
                    Fecha = siniestro1003.Fecha.AddDays(-10)
                },
                new()
                {
                    SiniestroId = siniestro1003.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    UsuarioId = usuarioMaria.Id,
                    Fecha = siniestro1003.Fecha.AddDays(-5)
                },
                new()
                {
                    SiniestroId = siniestro1003.Id,
                    SiniestroEstadoId = estadoAprobado.Id,
                    UsuarioId = usuarioJuan.Id,
                    Fecha = siniestro1003.Fecha
                },

                // Historial Siniestro 1004
                new()
                {
                    SiniestroId = siniestro1004.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    UsuarioId = usuarioJuan.Id,
                    Fecha = siniestro1004.Fecha.AddDays(-10)
                },
                new()
                {
                    SiniestroId = siniestro1004.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    UsuarioId = usuarioMaria.Id,
                    Fecha = siniestro1004.Fecha.AddDays(-5)
                },
                new()
                {
                    SiniestroId = siniestro1004.Id,
                    SiniestroEstadoId = estadoRechazado.Id,
                    UsuarioId = usuarioMaria.Id,
                    Fecha = siniestro1004.Fecha
                },

                // Historial Siniestro 1005
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoRecibido.Id,
                    UsuarioId = usuarioJuan.Id,
                    Fecha = siniestro1005.Fecha.AddDays(-30)
                },
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoEnAnalisis.Id,
                    UsuarioId = usuarioMaria.Id,
                    Fecha = siniestro1005.Fecha.AddDays(-20)
                },
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoAprobado.Id,
                    UsuarioId = usuarioMaria.Id,
                    Fecha = siniestro1005.Fecha.AddDays(-10)
                },
                new()
                {
                    SiniestroId = siniestro1005.Id,
                    SiniestroEstadoId = estadoCerrado.Id,
                    UsuarioId = usuarioJuan.Id,
                    Fecha = siniestro1005.Fecha
                }
            };

            await context.SiniestroEstadoHistoriales.AddRangeAsync(historiales);
            await context.SaveChangesAsync();
        }
    }
}

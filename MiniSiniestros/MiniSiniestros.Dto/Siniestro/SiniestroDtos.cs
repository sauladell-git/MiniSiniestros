using MiniSiniestros.Dto.Empleador;
using MiniSiniestros.Dto.Prestador;
using MiniSiniestros.Dto.Trabajador;

namespace MiniSiniestros.Dto.Siniestro
{
    public class SiniestroEstadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class SiniestroEstadoHistorialDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int SiniestroEstadoId { get; set; }
        public string SiniestroEstadoNombre { get; set; } = string.Empty;
    }

    public class SiniestroDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; } = string.Empty;

        public EmpleadorDto? Empleador { get; set; }
        public TrabajadorDto? Trabajador { get; set; }
        public SiniestroEstadoDto? SiniestroEstado { get; set; }

        public List<PrestadorDto> Prestadores { get; set; } = new();
        public List<SiniestroEstadoHistorialDto> HistorialEstados { get; set; } = new();
    }

    public class CreateSiniestroDto
    {
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public int SiniestroEstadoId { get; set; }
        public List<int> PrestadorIds { get; set; } = new();

        public string CuilEmpleador { get; set; } = string.Empty;
        public string CuilTrabajador { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }

    public class CambiarEstadoSiniestroDto
    {
        public int NuevoEstadoId { get; set; }
    }

    public class AsignarPrestadorSiniestroDto
    {
        public int PrestadorId { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using MiniSiniestros.Common.Paging;
using MiniSiniestros.Dto.Empleador;
using MiniSiniestros.Dto.Prestador;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Dto.Trabajador;

namespace MiniSiniestros.ViewModels.Siniestros
{
    public class SiniestroFilterViewModel
    {
        public string? Cuit { get; set; }
        public string? Cuil { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? SiniestroEstadoId { get; set; }
        public string? SortBy { get; set; } = "fecha";
        public bool IsDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SiniestroItemViewModel
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; } = string.Empty;

        public string EmpleadorRazonSocial { get; set; } = string.Empty;
        public string EmpleadorCuit { get; set; } = string.Empty;
        public string EmpleadorInfo => string.IsNullOrWhiteSpace(EmpleadorRazonSocial) ? EmpleadorCuit : $"{EmpleadorRazonSocial} ({EmpleadorCuit})";

        public string TrabajadorNombreCompleto { get; set; } = string.Empty;
        public string TrabajadorCuil { get; set; } = string.Empty;
        public string TrabajadorInfo => string.IsNullOrWhiteSpace(TrabajadorNombreCompleto) ? TrabajadorCuil : $"{TrabajadorNombreCompleto} ({TrabajadorCuil})";

        public string EstadoNombre { get; set; } = string.Empty;
        public int SiniestroEstadoId { get; set; }
    }

    public class SiniestroListViewModel
    {
        public SiniestroFilterViewModel Filter { get; set; } = new();
        public PagedResponse<SiniestroItemViewModel> Siniestros { get; set; } = new(new List<SiniestroItemViewModel>(), 1, 10, 0);
        public List<SelectListItem> EstadosSelectList { get; set; } = new();
    }

    public class SiniestroDetailViewModel
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
}

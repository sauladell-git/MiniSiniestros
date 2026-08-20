using MiniSiniestros.Common.Paging;

namespace MiniSiniestros.Dto.Siniestro
{
    public class SiniestroFilterRequest : PagedRequest
    {
        public string? Cuit { get; set; }
        public string? Cuil { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? SiniestroEstadoId { get; set; }
    }
}

namespace MiniSiniestros.Entities
{
    public class SiniestroEstadoHistorial
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int SiniestroId { get; set; }
        public Siniestro Siniestro { get; set; } = null!;
        public int SiniestroEstadoId { get; set; }
        public SiniestroEstado SiniestroEstado { get; set; } = null!;
 
    }
}

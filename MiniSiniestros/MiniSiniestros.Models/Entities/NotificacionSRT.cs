namespace MiniSiniestros.Entities
{
    public class NotificacionSRT
    {
        public int Id { get; set; }
        public int SiniestroId { get; set; }

        public Siniestro Siniestro { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public int Intentos { get; set; }
    }
}

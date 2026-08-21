namespace MiniSiniestros.Dto.Str
{
    public class SrtPayloadDto
    {
        public int SiniestroId { get; set; }
        public DateTime FechaAprobacion { get; set; }
        public string Estado { get; set; } = "Aprobado";
    }

    public class NotificacionSrtDto
    {
        public int Id { get; set; }
        public int SiniestroId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public int Intentos { get; set; }
    }
}

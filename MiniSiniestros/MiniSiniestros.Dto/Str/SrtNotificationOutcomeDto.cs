namespace MiniSiniestros.Dto.Str
{
    public class SrtNotificationOutcomeDto
    {
        public bool Exitoso { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public int Intentos { get; set; }
        public string? CodigoTransaccion { get; set; }
    }
}

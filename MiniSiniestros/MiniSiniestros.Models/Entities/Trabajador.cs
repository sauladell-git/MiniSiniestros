namespace MiniSiniestros.Entities
{
    public class Trabajador
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int EmpleadorId { get; set; }
        public Empleador Empleador { get; set; } = null!;
        public string Cuil { get; set; } = string.Empty;
    }
}

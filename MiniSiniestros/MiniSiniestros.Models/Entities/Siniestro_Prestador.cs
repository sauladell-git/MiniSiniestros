namespace MiniSiniestros.Entities
{
    public class Siniestro_Prestador
    {
        public int Id { get; set; }
        public int SiniestroId { get; set; }
        public Siniestro Siniestro { get; set; } = null!;
        public int PrestadorId { get; set; }
        public Prestador Prestador { get; set; } = null!;
    }
}

namespace MiniSiniestros.Entities
{
    public class Usuario_Rol
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;
    }
}

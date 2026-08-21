namespace MiniSiniestros.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<Usuario_Rol> UsuarioRoles { get; set; } = new List<Usuario_Rol>();
    }
}

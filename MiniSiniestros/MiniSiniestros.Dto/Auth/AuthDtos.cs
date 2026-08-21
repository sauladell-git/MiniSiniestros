namespace MiniSiniestros.Dto.Auth
{
    public class LoginDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}

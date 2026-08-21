using MiniSiniestros.Common.Responses;

namespace MiniSiniestros.Common.Constants
{
    public static class SiniestroErrorConstants
    {
        public static ValidationError SiniestroNotFound => new("SIN_NOT_FOUND", "Siniestro no encontrado.");
        public static ValidationError NumeroDuplicado => new("SIN_NUMERO_DUPLICADO", "Ya existe un siniestro registrado con ese número.");
        public static ValidationError CuitInvalido => new("CUIT_INVALIDO", "El CUIT del empleador debe ser un texto numérico de 11 dígitos.");
        public static ValidationError CuilInvalido => new("CUIL_INVALIDO", "El CUIL del trabajador debe ser un texto numérico de 11 dígitos.");
        public static ValidationError EmpleadorNotFound => new("EMPLEADOR_NOT_FOUND", "Empleado No Encontrado.");
        public static ValidationError TrabajadorNotFound => new("TRABAJADOR_NOT_FOUND", "Trabajador No Encontrado.");
        public static ValidationError EstadoNoDisponible => new("ESTADO_NO_DISPONIBLE", "Estado no permitido.");
        public static ValidationError UsuarioNotFound => new("USUARIO_NOT_FOUND", "Usuario no encontrado.");
        public static ValidationError PrestadorNotFound => new("PRESTADOR_NOT_FOUND", "Prestador no encontrado.");
        public static ValidationError PrestadorYaAsignado => new("PRESTADOR_YA_ASIGNADO", "El prestador ya se encuentra asignado a este siniestro.");
        public static ValidationError TrabajadorNoPerteneceAEmpleador => new("TRAB_NO_PERTENECE_EMP", "El trabajador no pertenece al empleador especificado.");
        public static ValidationError CredencialesInvalidas => new("CREDENCIALES_INVALIDAS", "Nombre de usuario o contraseña incorrectos.");
        public static ValidationError SystemError => new("SYS_ERROR", "Ocurrió un error inesperado en el sistema.");
    }
}

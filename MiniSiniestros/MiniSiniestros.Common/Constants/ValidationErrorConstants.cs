using MiniSiniestros.Common.Responses;

namespace MiniSiniestros.Common.Constants
{
    public static class ValidationErrorConstants
    {
        public static ValidationError CuilError => new("CUIL_ERROR", "Cuil Invalido");
        public static ValidationError CuitError => new("CUIT_ERROR", "Cuit Invalido");

        public static ValidationError EmpleadorNotFound => new("EMPLEADOR_NOT_FOUND", "Empleado No Encontrado");
        public static ValidationError TrabajadorNotFound => new("TRABAJADOR_NOT_FOUND", "Trabajador No Encontrado");
        public static ValidationError EstadoNoPermitido => new("ESTADO_NO_PERMITIDO", "Estado no permitido");
    }
}

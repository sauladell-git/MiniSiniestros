using MiniSiniestros.Common.Responses;

namespace MiniSiniestros.Common.Constants
{
    public static class EmpleadorErrorConstants
    {
        public static ValidationError EmpleadorNotFound => new("EMP_NOT_FOUND", "Empleado No Encontrado.");
    }
}

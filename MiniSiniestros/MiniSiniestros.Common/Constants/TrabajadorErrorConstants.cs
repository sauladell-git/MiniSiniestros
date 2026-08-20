using MiniSiniestros.Common.Responses;

namespace MiniSiniestros.Common.Constants
{
    public static class TrabajadorErrorConstants
    {
        public static ValidationError TrabajadorNotFound => new("TRAB_NOT_FOUND", "Trabajador No Encontrado.");
    }
}

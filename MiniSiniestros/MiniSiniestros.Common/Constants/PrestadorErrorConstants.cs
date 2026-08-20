using MiniSiniestros.Common.Responses;

namespace MiniSiniestros.Common.Constants
{
    public static class PrestadorErrorConstants
    {
        public static ValidationError PrestadorNotFound => new("PREST_NOT_FOUND", "Prestador No Encontrado.");
    }
}

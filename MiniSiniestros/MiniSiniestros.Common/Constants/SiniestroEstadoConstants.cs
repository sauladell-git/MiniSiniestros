namespace MiniSiniestros.Common.Constants
{
    public static class SiniestroEstadoConstants
    {
        public const int RecibidoId = 1;
        public const string Recibido = "Recibido";

        public const int EnAnalisisId = 2;
        public const string EnAnalisis = "EnAnalisis";

        public const int AprobadoId = 3;
        public const string Aprobado = "Aprobado";

        public const int RechazadoId = 4;
        public const string Rechazado = "Rechazado";

        public const int CerradoId = 5;
        public const string Cerrado = "Cerrado";

        public static IReadOnlyList<(int Id, string Nombre)> GetAllEstados()
        {
            return new List<(int Id, string Nombre)>
            {
                (RecibidoId, Recibido),
                (EnAnalisisId, EnAnalisis),
                (AprobadoId, Aprobado),
                (RechazadoId, Rechazado),
                (CerradoId, Cerrado)
            };
        }
    }
}

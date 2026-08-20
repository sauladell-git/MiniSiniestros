using MiniSiniestros.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSiniestros.Entities
{
    public class SiniestroEstadoHistorial
    {
        public int Id   { get; set; }
        public DateTime Fecha { get; set; }
        public int SiniestroId { get; set; }
        public Siniestro Siniestro { get; set; }
        public int SiniestroEstadoId { get; set; }
        public SiniestroEstado SiniestroEstado { get; set; }

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }
    }
}

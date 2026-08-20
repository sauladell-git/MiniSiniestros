using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSiniestros.Entities
{
    public class Siniestro
    { public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public int EmpleadorId { get; set; }
        public Empleador Empleador { get; set; }
       
        public int TrabajadorId { get; set; }

        public Trabajador Trabajador { get; set; }
        public int SiniestroEstadoId { get; set; }
        public SiniestroEstado SiniestroEstado { get; set; }
    }
}

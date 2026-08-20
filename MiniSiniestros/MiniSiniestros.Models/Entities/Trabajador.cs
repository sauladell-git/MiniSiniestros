using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSiniestros.Entities
{
    public class Trabajador
    {    public  int Id          { get; set; }
        public   string Nombre { get; set; }
        public   string Apellido { get; set; }

        public   int EmpleadorId { get; set; }

        public Empleador Empleador { get; set; }
        public string   Cuil     { get; set; }
    }
}

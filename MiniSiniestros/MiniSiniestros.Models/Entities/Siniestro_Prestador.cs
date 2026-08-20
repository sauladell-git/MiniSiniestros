using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSiniestros.Entities
{
    public class Siniestro_Prestador
    {
         public int Id               { get; set; }

         public int SiniestroId     { get; set; }

        public Siniestro  Siniestro { get; set; }

        public int PrestadorId      { get; set; }

        public Prestador Prestador { get; set; }


    }
}

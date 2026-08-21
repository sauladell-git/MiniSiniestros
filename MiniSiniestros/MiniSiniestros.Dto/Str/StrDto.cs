using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSiniestros.Dto.Str
{
   

        public class SrtPayloadDto
    {
        public int SiniestroId { get; set; }
        public DateTime FechaAprobacion { get; set; }
        public string Estado { get; set; } = "Aprobado";
    }
    
}

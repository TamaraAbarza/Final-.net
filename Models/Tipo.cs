using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABM_inmobiliaria.Models
{
    public class Tipo
    {

        public int Id { get; set; }
        public string TipoInmueble { get; set; } ="";

        public override string ToString()
        {
            return TipoInmueble;
        }

    }


}
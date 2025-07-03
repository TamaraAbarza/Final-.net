using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ABM_inmobiliaria.Models
{
    public class Inmueble
    {


        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Direccion { get; set; } = "";
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public int Ambientes { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public int Superficie { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public double Latitud { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public double Longitud { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public int IdPropietario { get; set; }

        public Propietario? Propietario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public int IdTipo { get; set; }
        public Tipo? Tipo { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public string Uso { get; set; } = "";
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public double Precio { get; set; }
        public bool Disponible { get; set; }




        public override string ToString()
        {
            return $"Direccion: {Direccion}, {Ambientes} Ambientes";
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ABM_inmobiliaria.Models
{
    public class Inquilino
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "El campo Apellido es obligatorio")]
        public string? Apellido { get; set; }
        //[Required(ErrorMessage = "El campo Telefono es obligatorio")]
        public string? Telefono { get; set; }
        //[Required(ErrorMessage = "El campo Email es obligatorio")]
        //[EmailAddress(ErrorMessage = "El campo Email no tiene un formato válido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El campo Dni es obligatorio")]
        public string? Dni { get; set; }
        public override string ToString()
        {
            return $"{Nombre} {Apellido}";
        }
    }
}
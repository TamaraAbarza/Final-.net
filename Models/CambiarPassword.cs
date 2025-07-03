using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ABM_inmobiliaria.Models
{
    public class CambiarPassword
    {
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password actual")]
        public string PasswordActual { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva password")]
        public string NuevaPassword { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirma nueva password")]
        public string ConfirmarPassword { get; set; }
    }
}
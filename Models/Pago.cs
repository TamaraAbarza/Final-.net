using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABM_inmobiliaria.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int NumeroPago { get; set; }
        public DateTime Fecha { get; set; }
        public string? Detalle { get; set; }
        public double Importe { get; set; }
        public bool Estado { get; set; }
        public int IdContrato { get; set; }
        public Contrato? Contrato { get; set; }
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
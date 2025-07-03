using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABM_inmobiliaria.Models
{
    public class Auditoria
    {
        public int Id { get; set; }
        
        public int IdUsuario {get;set;}

        public Usuario? Usuario {get;set;}

        public int IdEntidad {get;set;}

        public String? Entidad {get;set;}

        public DateTime FechaAccion{get;set;}
        
        public Boolean Accion {get;set;}
        
        public String? Descripcion {get;set;}



        public Auditoria(int id,int idUsuario , int idEntidad, String entidad, DateTime fechaAccion, Boolean accion, String descripcion){
            Id = id;
            IdUsuario = idUsuario;
            //Usuario = usuario;
            IdEntidad = idEntidad;
            Entidad = entidad;
            FechaAccion = fechaAccion;
            Accion = accion;
            Descripcion = descripcion;
        }

        public Auditoria(int id,int idUsuario, Usuario usuario , int idEntidad, String entidad, DateTime fechaAccion, Boolean accion, String descripcion){
            Id = id;
            IdUsuario = idUsuario;
            Usuario = usuario;
            IdEntidad = idEntidad;
            Entidad = entidad;
            FechaAccion = fechaAccion;
            Accion = accion;
            Descripcion = descripcion;
        }

        public Auditoria(){

        }
    }
}
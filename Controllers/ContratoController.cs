using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ABM_inmobiliaria.Models;
//using ZstdSharp.Unsafe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZstdSharp.Unsafe;

namespace ABM_inmobiliaria.Controllers
{
    public class ContratoController : Controller
    {
        private readonly ILogger<InmuebleController> _logger;
        RepositorioContrato rc = new RepositorioContrato();
        private RepositorioPropietario rp = new RepositorioPropietario();
        private RepositorioTipo rt = new RepositorioTipo();
        private RepositorioInquilino ri = new RepositorioInquilino();
        private RepositorioInmueble rinm = new RepositorioInmueble();
        private RepositorioUsuario ru = new RepositorioUsuario();

        private RepositorioAuditoria ra = new RepositorioAuditoria();
        private RepositorioPago repositorioPago = new RepositorioPago();

        public ContratoController(ILogger<InmuebleController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index(DateTime? fechaInicio, DateTime? fechaFin, int? idInmueble)
        {
            try
            {
                var contratos = rc.GetContratos(fechaInicio, fechaFin, idInmueble);
                ViewBag.FechaInicio =
                    fechaInicio?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.FechaFin =
                    fechaFin?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.Inmuebles = rinm.GetInmuebles();

                //auditoria
                var auditoriasPorContrato = new Dictionary<int, IList<Auditoria>>();
                foreach (var contrato in contratos)
                {
                    auditoriasPorContrato[contrato.Id] = ra.GetAuditoriasPorEntidad(contrato.Id, "contrato");
                }
                ViewBag.AuditoriasPorContrato = auditoriasPorContrato;

                return View(contratos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de contratos");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        public IActionResult Insertar()
        {
            ViewBag.Propietarios = rp.GetPropietarios();
            ViewBag.Inquilinos = ri.GetInquilinos();
            ViewBag.inmuebles = rinm.GetInmuebles();

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Insertar(Contrato contrato)
        {
            try
            {
                // Verificar si la fecha de inicio es mayor o igual que la fecha de fin
                if (contrato.FechaInicio >= contrato.FechaFin)
                {
                    throw new ArgumentException(
                        "Error: La fecha de inicio no puede ser mayor o igual que la fecha de fin."
                    );
                }

                // Verificar si el inmueble está ocupado en otro contrato entre las fechas proporcionadas
                bool inmuebleOcupado = rc.InmuebleOcupadoEnOtroContrato(
                    contrato.IdInmueble,
                    contrato.FechaInicio,
                    contrato.Id
                );
                if (inmuebleOcupado)
                {
                    throw new ArgumentException(
                        "Error: El inmueble ya está ocupado en otro contrato durante las fechas proporcionadas."
                    );
                }

                contrato.Inmueble = rinm.GetInmueble(contrato.IdInmueble);

                //Verificar que el inmueble este disponible
                if (contrato.Inmueble != null && !contrato.Inmueble.Disponible)
                {
                    throw new ArgumentException(
                        "Error: El inmueble seleccionado no está disponible actualmente"
                    );
                }

                // Si no hay errores, insertar el contrato
                if (ModelState.IsValid)
                {
                    //Obtengo el propietario perteneciente al inmueble
                    contrato.IdPropietario = contrato.Inmueble.IdPropietario;
                    //Obtengo el usuario que realizo el contrato
                    if (User.Identity.IsAuthenticated)
                    {
                        var usuario = ru.GetUsuarioEmail(User.FindFirst(ClaimTypes.Email).Value);
                        ///Obtengo el usuario que inicio sesion desde la claim
                        contrato.Usuario = usuario;
                        contrato.IdUsuario = usuario.Id;
                    }
                    int id = rc.InsertarContrato(contrato);

                    //cambiar estado del inmueble a no disponible
                    rinm.ModificarEstadoInmueble(contrato.Inmueble.Id, false);

                    //guardar info en auditoria
                    var auditoria = new Auditoria
                    {
                        IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                        IdEntidad = id,
                        Entidad = "Contrato",
                        FechaAccion = DateTime.Today,
                        Accion = true,
                        Descripcion = "Creación de contrato"
                    };

                    ra.InsertarAuditoria(auditoria);

                    TempData["Mensaje"] = "El contrato se ha creado correctamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                ViewBag.Propietarios = rp.GetPropietarios();
                ViewBag.Inquilinos = ri.GetInquilinos();
                ViewBag.inmuebles = rinm.GetInmuebles();

                return View(contrato);
            }
            catch (ArgumentException ex)
            {
                // Capturar la excepción y mostrar el mensaje de error
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "error";
                ViewBag.Propietarios = rp.GetPropietarios();
                ViewBag.Inquilinos = ri.GetInquilinos();
                ViewBag.inmuebles = rinm.GetInmuebles();

                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar el contrato");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        public IActionResult Actualizar(int id)
        {
            var contrato = rc.GetContrato(id);
            if (contrato == null)
            {
                return NotFound();
            }
            ViewBag.Propietarios = rp.GetPropietarios();
            ViewBag.Inquilinos = ri.GetInquilinos();
            ViewBag.Inmuebles = rinm.GetInmuebles();
            return View(contrato);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Actualizar(Contrato contrato)
        {
            try
            {
                // Obtener el contrato original de la base de datos
                var contratoOriginal = rc.GetContrato(contrato.Id);
                if (contratoOriginal == null)
                {
                    throw new ArgumentException("Error: Contrato no encontrado para actualizar.");
                }
                // Validaciones ----------------------------------
                // Verificar si la fecha de inicio es mayor o igual que la fecha de fin
                if (contrato.FechaInicio >= contrato.FechaFin)
                {
                    throw new ArgumentException(
                        "Error: La fecha de inicio no puede ser mayor o igual que la fecha de fin."
                    );
                }

                // Verificar si el inmueble está ocupado en otro contrato entre las fechas proporcionadas
                bool inmuebleOcupado = rc.InmuebleOcupadoEnOtroContrato(
                    contrato.IdInmueble,
                    contrato.FechaInicio,
                    contrato.Id
                );
                if (inmuebleOcupado)
                {
                    throw new ArgumentException(
                        "Error: El inmueble ya está ocupado en otro contrato durante las fechas proporcionadas."
                    );
                }

                // Obtener el inmueble actual para verificar disponibilidad
                contrato.Inmueble = rinm.GetInmueble(contrato.IdInmueble);
                if (contrato.Inmueble != null && !contrato.Inmueble.Disponible && contrato.IdInmueble != contratoOriginal.IdInmueble)
                {
                    // Solo lanzar error si el inmueble no está disponible Y es diferente al original
                    // Si es el mismo inmueble y ya está no disponible por este contrato, no es un error.
                    throw new ArgumentException("Error: El inmueble seleccionado no está disponible actualmente.");
                }

                if (ModelState.IsValid)
                {
                    // Obtengo el propietario perteneciente al inmueble
                    contrato.IdPropietario = contrato.Inmueble.IdPropietario;

                    // 2. Lógica para cambiar el estado de los inmuebles si IdInmueble ha cambiado
                    if (contrato.IdInmueble != contratoOriginal.IdInmueble)
                    {
                        // El inmueble ha cambiado:
                        // a) Poner el inmueble ANTERIOR como DISPONIBLE (true)
                        rinm.ModificarEstadoInmueble(contratoOriginal.IdInmueble, true);

                        // b) Poner el inmueble NUEVO como NO DISPONIBLE (false)
                        rinm.ModificarEstadoInmueble(contrato.IdInmueble, false);
                    }
                    // Si el inmueble no cambió, no es necesario modificar su estado aquí,

                    rc.ActualizarContrato(contrato);

                    // Guardar info en auditoria
                    var auditoria = new Auditoria
                    {
                        IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                        IdEntidad = contrato.Id,
                        Entidad = "Contrato",
                        FechaAccion = DateTime.Today,
                        Accion = false, // false para actualización
                        Descripcion = "Actualización de contrato"
                    };
                    ra.InsertarAuditoria(auditoria);

                    TempData["Mensaje"] = "El contrato se ha actualizado correctamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                ViewBag.Propietarios = rp.GetPropietarios();
                ViewBag.Inquilinos = ri.GetInquilinos();
                ViewBag.Inmuebles = rinm.GetInmuebles();
                return View(contrato);
            }
            catch (ArgumentException ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "error";
                ViewBag.Propietarios = rp.GetPropietarios();
                ViewBag.Inquilinos = ri.GetInquilinos();
                ViewBag.Inmuebles = rinm.GetInmuebles();
                // Es importante devolver la vista con el modelo actual para que el usuario vea los datos que intentó enviar
                return View(contrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el contrato");
                return RedirectToAction("Error");
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var contrato = rc.GetContrato(id);
                rc.EliminarContrato(id);
                //cambiar estado del inmueble a disponible
                rinm.ModificarEstadoInmueble(contrato.Inmueble.Id, true);

                // Insertar entrada en la tabla de auditoría
                var usuario = ru.GetUsuarioEmail(User.FindFirst(ClaimTypes.Email).Value);
                ///Obtengo el usuario que inicio sesion desde la claim
                var idUsuario = usuario.Id;
                int idEntidad = id; // El ID de la entidad eliminada (en este caso, el contrato)
                bool entidad = true; // El nombre de la entidad eliminada
                DateTime fechaAccion = DateTime.Now; // La fecha y hora actual

                // ra.InsertarAuditoria(idUsuario, id, entidad, fechaAccion);
                //ra.InsertarAuditoria(id);
                TempData["Mensaje"] = $"Se eliminó correctamente el contrato";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar el contrato");
                // Puedes analizar el ex.Number o ex.ErrorCode para mensajes más específicos si lo deseas
                if (ex.Message.Contains("foreign key constraint fails"))
                {
                    // Mensaje específico para el error de clave foránea
                    TempData["Mensaje"] = "No se puede eliminar el contrato porque tiene pagos asociados.";
                }
                else
                {
                    // Mensaje genérico para otros errores de MySQL
                    TempData["Mensaje"] = "Ocurrió un error de base de datos al intentar eliminar el contrato.";
                }
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index"); // Redirige a Index para mostrar el TempData
            }
        }

        //Adicional

        [HttpPost]
        public IActionResult CalcularMulta(int id, DateTime fechaTerminacion)
        {
            var contrato = rc.GetContrato(id);
            if (contrato == null)
            {
                return NotFound();
            }

            double multa = CalcularMulta(contrato, fechaTerminacion);
            return Json(new { multa });
        }

        //TERMINAR  CONTRATO
        [HttpPost]
        public IActionResult Terminar(int id, DateTime fechaTerminacion)
        {
            var contrato = rc.GetContrato(id);
            if (contrato == null)
            {
                return NotFound();
            }

            if (contrato.FechaTerminacion.HasValue)
            {
                return BadRequest("El contrato ya ha sido terminado anticipadamente.");
            }

            double multa = CalcularMulta(contrato, fechaTerminacion);

            // Registrar el pago de la multa
            var pago = new Pago
            {
                IdContrato = contrato.Id,
                Fecha = fechaTerminacion,
                Importe = multa,
                Detalle = "Finalización de contrato",
                Contrato = contrato
            };
            pago.NumeroPago = repositorioPago.ObtenerNumeroPago(pago.IdContrato);
            pago.IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            contrato.FechaTerminacion = fechaTerminacion;
            contrato.MultaTerminacion = multa;
            contrato.Vigente = false;

            var auditoria = new Auditoria
            {
                IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                IdEntidad = contrato.Id,
                Entidad = "Contrato",
                FechaAccion = fechaTerminacion,
                Accion = false,
                Descripcion = "Finalización de contrato"
            };

            rc.ActualizarContrato(contrato);
            repositorioPago.InsertarPago(pago);
            ra.InsertarAuditoria(auditoria);
            //cambiar estado del inmueble a disponible
            rinm.ModificarEstadoInmueble(contrato.Inmueble.Id, true);

            // Redirigir a la vista intermedia
            TempData["Mensaje"] = $"Contrato terminado anticipadamente. Multa: {multa}.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("Index");
        }

        private double CalcularMulta(Contrato contrato, DateTime fechaTerminacion)
        {
            // Verificar si tiene meses de alquiler adeudados
            double montoTotal = 0;
            var mesesTotales = (int)((fechaTerminacion - contrato.FechaInicio).TotalDays / 30);
            var mesesPagos = rc.ObtenerPagos(contrato.Id).Count;
            if (mesesPagos < mesesTotales)
            {
                montoTotal = (mesesTotales - mesesPagos) * contrato.MontoMensual;
            }

            // Calcular la multa
            var tiempoTranscurrido = (fechaTerminacion - contrato.FechaInicio).TotalDays;
            var tiempoTotal = (contrato.FechaFin - contrato.FechaInicio).TotalDays;
            var mitadTiempo = tiempoTotal / 2;

            double multa = montoTotal;
            if (tiempoTranscurrido < mitadTiempo)
            {
                multa += contrato.MontoMensual * 2;
            }
            else
            {
                multa += contrato.MontoMensual;
            }

            return multa;
        }

        [HttpGet]
        public IActionResult Renovar(int id)
        {
            var contrato = rc.GetContrato(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        [HttpPost]
        public IActionResult Renovar(
            int id,
            DateTime fechaInicio,
            DateTime fechaFin,
            double montoMensual
        )
        {
            try
            {
                var contrato = rc.GetContrato(id);

                // Verificar si la fecha de inicio es mayor o igual que la fecha de fin
                if (contrato.FechaInicio >= contrato.FechaFin)
                {
                    throw new ArgumentException(
                        "Error: La fecha de inicio no puede ser mayor o igual que la fecha de fin."
                    );
                }

                // Verificar si el inmueble está ocupado en otro contrato entre las fechas proporcionadas
                bool inmuebleOcupado = rc.InmuebleOcupadoEnOtroContrato(
                    contrato.IdInmueble,
                    contrato.FechaInicio,
                    contrato.Id
                );
                if (inmuebleOcupado)
                {
                    throw new ArgumentException(
                        "Error: El inmueble ya está ocupado en otro contrato durante las fechas proporcionadas."
                    );
                }

                contrato.Inmueble = rinm.GetInmueble(contrato.IdInmueble);

                //Verificar que el inmueble este disponible
                if (contrato.Inmueble != null && !contrato.Inmueble.Disponible)
                {
                    throw new ArgumentException(
                        "Error: El inmueble seleccionado no está disponible actualmente"
                    );
                }

                // Si no hay errores, insertar el contrato
                if (ModelState.IsValid)
                {
                    if (contrato == null)
                    {
                        return NotFound();
                    }

                    var usuario = ru.GetUsuarioEmail(User.FindFirst(ClaimTypes.Email).Value);
                    ///Obtengo el usuario que inicio sesion desde la claim

                    var nuevoContrato = new Contrato
                    {
                        IdInquilino = contrato.IdInquilino,
                        IdPropietario = contrato.IdPropietario,
                        IdInmueble = contrato.IdInmueble,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        MontoMensual = montoMensual,
                        Vigente = true,
                        IdUsuario = usuario.Id
                    };

                    rc.InsertarContrato(nuevoContrato);
                    TempData["Mensaje"] = "El contrato se ha renovado correctamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
            }
            catch (ArgumentException ex)
            {
                // Capturar la excepción y mostrar el mensaje de error
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "error";
                ViewBag.Propietarios = rp.GetPropietarios();
                ViewBag.Inquilinos = ri.GetInquilinos();
                ViewBag.inmuebles = rinm.GetInmuebles();

                return RedirectToAction("renovar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al renovar el contrato");
                return RedirectToAction("renovar");
            }
            return RedirectToAction("index");
        }

        [Authorize]
        public IActionResult Insertar2(
            int? inmuebleId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null
        )
        {
            ViewBag.Propietarios = rp.GetPropietarios();
            ViewBag.Inquilinos = ri.GetInquilinos();
            //ViewBag.Inmuebles = rinm.GetInmuebles();

            // Pasar las fechas a la vista usando ViewBag
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            var inmueble = rinm.GetInmueble(inmuebleId ?? 0);
            // Pasar el inmueble seleccionado a la vista usando ViewBag
            ViewBag.Inmueble = inmueble;

            return View();
        }
    }
}

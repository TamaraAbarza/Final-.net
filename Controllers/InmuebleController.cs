using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ABM_inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZstdSharp.Unsafe;

namespace ABM_inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly ILogger<InmuebleController> _logger;
        RepositorioInmueble ri = new RepositorioInmueble();
        private RepositorioPropietario rp = new RepositorioPropietario();
        private RepositorioTipo rt = new RepositorioTipo();

        public InmuebleController(ILogger<InmuebleController> logger)
        {
            _logger = logger;
        }

       [Authorize]
public IActionResult Index(DateTime? fechaInicio = null, DateTime? fechaFin = null)
{
    try
    {
        var inmuebles = ri.GetInmuebles();

        ViewBag.Propietarios = rp.GetPropietarios();
        ViewBag.TiposInmueble = rt.GetTipo();
        ViewBag.FechaInicio = fechaInicio ?? DateTime.Today;
        ViewBag.FechaFin = fechaFin ?? DateTime.Today.AddMonths(1);

        return View(inmuebles);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener la lista de inmuebles");
        return RedirectToAction("Error");
    }
}

        [Authorize]
        public IActionResult Insertar()
        {
            ViewBag.Propietarios = rp.GetPropietarios();
            ViewBag.TiposInmueble = rt.GetTipo();
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Insertar(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ri.InsertarInmueble(inmueble);
                    TempData["Mensaje"] = "El inmueble se ha creado correctamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                ViewBag.Propietarios = rp.GetPropietarios();
                ViewBag.TiposInmueble = rt.GetTipo();
                return View(inmueble);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar el inmueble");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        public IActionResult Actualizar(int id)
        {
            var inmueble = ri.GetInmueble(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            ViewBag.Propietarios = rp.GetPropietarios();
            ViewBag.TiposInmueble = rt.GetTipo();
            return View(inmueble);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Actualizar(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ri.ActualizarInmueble(inmueble);
                    TempData["Mensaje"] = "El inmueble se ha actualizado correctamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                ViewBag.Propietarios = rp.GetPropietarios();
                ViewBag.TiposInmueble = rt.GetTipo();
                return View(inmueble);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el inmueble");
                return RedirectToAction("Error");
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                ri.EliminarInmueble(id);
                TempData["Mensaje"] = $"Se eliminó correctamente el inmueble";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el inmueble");

                TempData["Mensaje"] =
                    $"Error, es probable que el inmueble pertenezca a un contrato vigente";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("index");
            }
        }

        //LISTAR -------------------------------------------------------------------------------------
        [Authorize]
        public IActionResult InmueblesPorPropietario(int propietarioId, bool? disponible)
        {
            try
            {
                var inmuebles = ri.GetInmueblesPorPropietario(propietarioId, disponible);
                return PartialView("InmueblesTable", inmuebles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de inmuebles por propietario");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        public IActionResult Disponibles()
        {
            try
            {
                var inmuebles = ri.GetInmueblesDisponibles();
                return PartialView("InmueblesTable", inmuebles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de inmuebles disponibles");
                return RedirectToAction("Error");
            }
        }

        //EXTRA
        [Authorize]
        public IActionResult InmueblesDisponiblesPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                if (fechaInicio == DateTime.MinValue || fechaFin == DateTime.MinValue)
                {
                    throw new ArgumentException("Las fechas no pueden estar vacías");
                }

                _logger.LogInformation(
                    "Obteniendo inmuebles disponibles para el rango: {FechaInicio} - {FechaFin}",
                    fechaInicio,
                    fechaFin
                );

                var inmueblesDisponibles = ri.GetInmueblesDisponiblesPorFecha(fechaInicio , fechaFin);
                ViewBag.FechaInicio = fechaInicio;
                ViewBag.FechaFin = fechaFin;

                return PartialView("InmueblesTable", inmueblesDisponibles);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al obtener la lista de inmuebles disponibles por fecha"
                );
                return RedirectToAction("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}

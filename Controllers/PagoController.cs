using System.Security.Claims;
using ABM_inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ABM_inmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        private readonly ILogger<PagoController> _logger;
        RepositorioPago rp = new RepositorioPago();
        RepositorioContrato rc = new RepositorioContrato();
        RepositorioAuditoria ra = new RepositorioAuditoria();

        public PagoController(ILogger<PagoController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            try
            {
                var pagos = rp.GetPagos();
                ViewBag.Contratos = rc.GetContratos();

                // Cargar auditorías para cada pago
                var auditoriasPorPago = new Dictionary<int, IList<Auditoria>>();
                foreach (var pago in pagos)
                {
                    auditoriasPorPago[pago.Id] = ra.GetAuditoriasPorEntidad(pago.Id, "pago");
                }
                ViewBag.AuditoriasPorPago = auditoriasPorPago;

                return View(pagos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de pagos");
                return RedirectToAction("Error");
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                rp.EliminarPago(id);

                //guardar la info en auditoria
                var auditoria = new Auditoria
                {
                    IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    IdEntidad = id,
                    Entidad = "Pago",
                    FechaAccion = DateTime.Today,
                    Accion = false,
                    Descripcion = "Pago anulado"
                };
                ra.InsertarAuditoria(auditoria);

                TempData["Mensaje"] = $"Se eliminó correctamente el pago";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el pago");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        public IActionResult Insertar(int idContrato)
        {
            var contrato = rc.GetContrato(idContrato);
            var pago = new Pago { IdContrato = idContrato, Contrato = contrato };
            ViewBag.Contratos = rc.GetContratos();
            return View(pago);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Insertar(Pago pago)
        {
            try
            {
                if (pago.Importe <= 0)
                {
                    ModelState.AddModelError(
                        "Importe",
                        "El importe debe ser un número válido y mayor que cero."
                    );
                    ViewBag.Contratos = rc.GetContratos();
                }

                if (ModelState.IsValid)
                {
                    pago.NumeroPago = rp.ObtenerNumeroPago(pago.IdContrato);
                    pago.IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // id del usuario autenticado

                    int id = rp.InsertarPago(pago);
                    //guardar la info en auditoria
                    var auditoria = new Auditoria
                    {
                        IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                        IdEntidad = id,
                        Entidad = "Pago",
                        FechaAccion = DateTime.Today,
                        Accion = true,
                        Descripcion = "Nuevo pago"
                    };
                    ra.InsertarAuditoria(auditoria);

                    TempData["Mensaje"] = "Se realizó el pago correctamente";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                return View(pago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar el pago en la bd");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        public IActionResult Actualizar(int id)
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Actualizar(Pago pago)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    rp.ActualizarPago(pago);
                    TempData["Mensaje"] = "El pago se ha actualizado correctamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                return View(pago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el pago");
                return RedirectToAction("Error");
            }
        }

        //Adicional
        [Authorize]
        public IActionResult ListarPagosPorContrato(int idContrato)
        {
            try
            {
                // Si el idContrato es 0, mostrar todos los pagos
                if (idContrato == 0)
                {
                    return RedirectToAction("Index");
                }

                var pagos = rp.GetPagosPorContrato(idContrato);
                ViewBag.Contratos = rc.GetContratos();

                //auditoria
                var auditoriasPorPago = new Dictionary<int, IList<Auditoria>>();
                foreach (var pago in pagos)
                {
                    auditoriasPorPago[pago.Id] = ra.GetAuditoriasPorEntidad(pago.Id, "pago");
                }
                ViewBag.AuditoriasPorPago = auditoriasPorPago;

                return View("Index", pagos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de pagos por contrato");
                return RedirectToAction("Error");
            }
        }
    }
}

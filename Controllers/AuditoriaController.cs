using ABM_inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ABM_inmobiliaria.Controllers{
    public class AuditoriaController : Controller{

        private readonly ILogger<AuditoriaController> _logger;
        private RepositorioAuditoria ra = new RepositorioAuditoria();

        public AuditoriaController(ILogger<AuditoriaController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var auditorias = ra.GetAuditorias();
                return View(auditorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de contratos");
                return RedirectToAction("Error");
            }
        }
    }
}
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.ViewModels.Siniestros;
using MiniSiniestros.Web.Models;
using MiniSiniestros.Web.Services;

namespace MiniSiniestros.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISiniestroApiClient _apiClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ISiniestroApiClient apiClient, ILogger<HomeController> logger)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index([FromQuery] SiniestroFilterViewModel filter, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cargando vista principal de siniestros.");
            var response = await _apiClient.GetPagedSiniestrosAsync(filter, cancellationToken);

            if (!response.Success && response.Errors.Count > 0)
            {
                ViewBag.ErrorMessage = response.Errors[0].Message;
            }

            return View(response.Data ?? new SiniestroListViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

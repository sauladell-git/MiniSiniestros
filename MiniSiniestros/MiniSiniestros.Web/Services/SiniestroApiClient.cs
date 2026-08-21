using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Paging;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.ViewModels.Siniestros;

namespace MiniSiniestros.Web.Services
{
    public class SiniestroApiClient : ISiniestroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SiniestroApiClient> _logger;

        public SiniestroApiClient(HttpClient httpClient, ILogger<SiniestroApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<SiniestroListViewModel>> GetPagedSiniestrosAsync(SiniestroFilterViewModel filter, CancellationToken cancellationToken = default)
        {
            filter ??= new SiniestroFilterViewModel();

            var queryParams = new List<string>
            {
                $"pageNumber={filter.PageNumber}",
                $"pageSize={filter.PageSize}",
                $"isDescending={filter.IsDescending.ToString().ToLower()}"
            };

            if (!string.IsNullOrWhiteSpace(filter.Cuit))
                queryParams.Add($"cuit={Uri.EscapeDataString(filter.Cuit.Trim())}");

            if (!string.IsNullOrWhiteSpace(filter.Cuil))
                queryParams.Add($"cuil={Uri.EscapeDataString(filter.Cuil.Trim())}");

            if (filter.FechaDesde.HasValue)
                queryParams.Add($"fechaDesde={filter.FechaDesde.Value:yyyy-MM-dd}");

            if (filter.FechaHasta.HasValue)
                queryParams.Add($"fechaHasta={filter.FechaHasta.Value:yyyy-MM-dd}");

            if (filter.SiniestroEstadoId.HasValue && filter.SiniestroEstadoId.Value > 0)
                queryParams.Add($"siniestroEstadoId={filter.SiniestroEstadoId.Value}");

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(filter.SortBy.Trim())}");

            var requestUrl = $"api/siniestros?{string.Join("&", queryParams)}";
            _logger.LogInformation("Consultando API Siniestros desde WebClient: {RequestUrl}", requestUrl);

            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ServiceResponse<PagedResponse<SiniestroDto>>>(requestUrl, cancellationToken);
                
                if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
                {
                    _logger.LogWarning("Respuesta no exitosa de la API de Siniestros.");
                    return ServiceResponse<SiniestroListViewModel>.Fail(apiResponse?.Errors ?? new List<ValidationError> { SiniestroErrorConstants.SystemError });
                }

                var pagedData = apiResponse.Data;
                var itemsVm = pagedData.Items.Select(d => new SiniestroItemViewModel
                {
                    Id = d.Id,
                    Numero = d.Numero,
                    Fecha = d.Fecha,
                    Observaciones = d.Observaciones,
                    EmpleadorRazonSocial = d.Empleador?.RazonSocial ?? string.Empty,
                    EmpleadorCuit = d.Empleador?.Cuit ?? string.Empty,
                    TrabajadorNombreCompleto = d.Trabajador != null ? $"{d.Trabajador.Nombre} {d.Trabajador.Apellido}".Trim() : string.Empty,
                    TrabajadorCuil = d.Trabajador?.Cuil ?? string.Empty,
                    EstadoNombre = d.SiniestroEstado?.Nombre ?? string.Empty,
                    SiniestroEstadoId = d.SiniestroEstado?.Id ?? 0
                }).ToList();

                var pagedResponseVm = new PagedResponse<SiniestroItemViewModel>(
                    itemsVm,
                    pagedData.PageNumber,
                    pagedData.PageSize,
                    pagedData.TotalRecords);

                var viewModel = new SiniestroListViewModel
                {
                    Filter = filter,
                    Siniestros = pagedResponseVm,
                    EstadosSelectList = BuildEstadosSelectList(filter.SiniestroEstadoId)
                };

                return ServiceResponse<SiniestroListViewModel>.Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consumir la API de Siniestros en {RequestUrl}", requestUrl);
                
                var fallbackVm = new SiniestroListViewModel
                {
                    Filter = filter,
                    Siniestros = new PagedResponse<SiniestroItemViewModel>(new List<SiniestroItemViewModel>(), filter.PageNumber, filter.PageSize, 0),
                    EstadosSelectList = BuildEstadosSelectList(filter.SiniestroEstadoId)
                };

                return ServiceResponse<SiniestroListViewModel>.Fail(SiniestroErrorConstants.SystemError, $"No se pudo conectar con la API de Siniestros: {ex.Message}");
            }
        }

        private static List<SelectListItem> BuildEstadosSelectList(int? selectedEstadoId)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Todos los Estados --", Selected = !selectedEstadoId.HasValue || selectedEstadoId.Value == 0 }
            };

            foreach (var (id, nombre) in SiniestroEstadoConstants.GetAllEstados())
            {
                list.Add(new SelectListItem
                {
                    Value = id.ToString(),
                    Text = nombre,
                    Selected = selectedEstadoId.HasValue && selectedEstadoId.Value == id
                });
            }

            return list;
        }
    }
}

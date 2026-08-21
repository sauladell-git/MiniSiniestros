using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Enums;
using MiniSiniestros.Common.Paging;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Prestador;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class SiniestroService : ISiniestroService
    {
        private static readonly Regex CuitCuilRegex = new(@"^\d{11}$", RegexOptions.Compiled);

        private readonly IUoWData _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SiniestroService> _logger;
        private readonly IEmpleadorService _empleadorService;
        private readonly ITrabajadorService _trabajadorService;
        private readonly ISiniestroEstadoService _siniestroEstadoService;
        private readonly IPrestadorService _prestadorService;
        private readonly IStrNotificationService _strNotificationService;

        public SiniestroService(
            IUoWData unitOfWork,
            IMapper mapper,
            ILogger<SiniestroService> logger,
            IEmpleadorService empleadorService,
            ITrabajadorService trabajadorService,
            ISiniestroEstadoService siniestroEstadoService,
            IPrestadorService prestadorService,
            IStrNotificationService strNotificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _empleadorService = empleadorService ?? throw new ArgumentNullException(nameof(empleadorService));
            _trabajadorService = trabajadorService ?? throw new ArgumentNullException(nameof(trabajadorService));
            _siniestroEstadoService = siniestroEstadoService ?? throw new ArgumentNullException(nameof(siniestroEstadoService));
            _prestadorService = prestadorService ?? throw new ArgumentNullException(nameof(prestadorService));
            _strNotificationService = strNotificationService ?? throw new ArgumentNullException(nameof(strNotificationService));
        }

        public async Task<ServiceResponse<IReadOnlyList<SiniestroDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Obteniendo el listado completo de siniestros con detalles.");

            var siniestros = await _unitOfWork.Siniestros.GetAllConDetallesAsync(cancellationToken);
            var dtos = new List<SiniestroDto>();

            foreach (var entity in siniestros)
            {
                var dto = _mapper.Map<SiniestroDto>(entity);
                await LoadPrestadoresYHistorialAsync(dto, cancellationToken);
                dtos.Add(dto);
            }

            _logger.LogInformation("Se obtuvieron exitosamente {Count} siniestros.", dtos.Count);
            return ServiceResponse<IReadOnlyList<SiniestroDto>>.Ok(dtos, "Siniestros obtenidos correctamente.");
        }

        public async Task<ServiceResponse<SiniestroDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando siniestro con ID {SiniestroId}", id);

            var siniestro = await _unitOfWork.Siniestros.GetByIdConDetallesAsync(id, cancellationToken);
            if (siniestro == null)
            {
                _logger.LogWarning("Siniestro con ID {SiniestroId} no fue encontrado.", id);
                return ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.SiniestroNotFound);
            }

            var dto = _mapper.Map<SiniestroDto>(siniestro);
            await LoadPrestadoresYHistorialAsync(dto, cancellationToken);
            return ServiceResponse<SiniestroDto>.Ok(dto);
        }

        public async Task<ServiceResponse<PagedResponse<SiniestroDto>>> GetPagedAsync(SiniestroFilterRequest filter, CancellationToken cancellationToken = default)
        {
            filter ??= new SiniestroFilterRequest();

            _logger.LogInformation("Consultando lista paginada de siniestros. Página {PageNumber}, Tamaño {PageSize}, Filtros: CUIT={Cuit}, CUIL={Cuil}, Desde={Desde}, Hasta={Hasta}, Estado={EstadoId}, SortBy={SortBy}",
                filter.PageNumber, filter.PageSize, filter.Cuit, filter.Cuil, filter.FechaDesde, filter.FechaHasta, filter.SiniestroEstadoId, filter.SortBy);

            var (items, totalCount) = await _unitOfWork.Siniestros.GetPagedAsync(filter, cancellationToken);
            var dtos = new List<SiniestroDto>();

            foreach (var entity in items)
            {
                var dto = _mapper.Map<SiniestroDto>(entity);
                await LoadPrestadoresYHistorialAsync(dto, cancellationToken);
                dtos.Add(dto);
            }

            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var pagedResponse = new PagedResponse<SiniestroDto>(dtos, pageNumber, pageSize, totalCount);

            _logger.LogInformation("Siniestros paginados obtenidos exitosamente. Registros totales: {TotalRecords}, Registros devueltos: {Count}", totalCount, dtos.Count);
            return ServiceResponse<PagedResponse<SiniestroDto>>.Ok(pagedResponse);
        }

        public async Task<ServiceResponse<SiniestroDto>> CreateAsync(CreateSiniestroDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Iniciando solicitud de creación de nuevo siniestro.");

            // Validar formato CUIT Empleador con Expresión Regular (exclusivamente 11 dígitos numéricos sin guiones)
            var cuitClean = dto.CuilEmpleador?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cuitClean) || !CuitCuilRegex.IsMatch(cuitClean))
            {
                _logger.LogWarning("Validación fallida: CUIT de empleador es inválido '{Cuit}'. Debe ser un texto numérico de 11 dígitos sin guiones.", dto.CuilEmpleador);
                return ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.CuitInvalido);
            }

            // Validar formato CUIL Trabajador con Expresión Regular (exclusivamente 11 dígitos numéricos sin guiones)
            var cuilClean = dto.CuilTrabajador?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cuilClean) || !CuitCuilRegex.IsMatch(cuilClean))
            {
                _logger.LogWarning("Validación fallida: CUIL de trabajador es inválido '{Cuil}'. Debe ser un texto numérico de 11 dígitos sin guiones.", dto.CuilTrabajador);
                return ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.CuilInvalido);
            }

            // 1. Obtener Empleador por CUIT desde IEmpleadorService
            var empleadorRes = await _empleadorService.GetByCuitAsync(cuitClean, cancellationToken);
            if (!empleadorRes.Success || empleadorRes.Data == null)
            {
                _logger.LogWarning("Validación fallida: Empleador con CUIT '{Cuit}' no existe.", cuitClean);
                return ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.EmpleadorNotFound);
            }
            var empleador = empleadorRes.Data;

            // 2. Obtener Trabajador por CUIL desde ITrabajadorService
            var trabajadorRes = await _trabajadorService.GetByCuilAsync(cuilClean, cancellationToken);
            if (!trabajadorRes.Success || trabajadorRes.Data == null)
            {
                _logger.LogWarning("Validación fallida: Trabajador con CUIL '{Cuil}' no existe.", cuilClean);
                return ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.TrabajadorNotFound);
            }
            var trabajador = trabajadorRes.Data;

            // 3. Verificar que el trabajador pertenezca al empleador usando ITrabajadorService.ExistePorTrabajadorYEmpleadorAsync
            var relacionRes = await _trabajadorService.ExistePorTrabajadorYEmpleadorAsync(trabajador.Id, empleador.Id, cancellationToken);
            if (!relacionRes.Success || !relacionRes.Data)
            {
                _logger.LogWarning("Validación fallida: El trabajador con CUIL '{Cuil}' no pertenece al empleador con CUIT '{Cuit}'.", cuilClean, cuitClean);
                return ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.TrabajadorNoPerteneceAEmpleador);
            }


            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var siniestro = _mapper.Map<Siniestro>(dto);
                siniestro.EmpleadorId = empleador.Id;
                siniestro.TrabajadorId = trabajador.Id;
                siniestro.Fecha = System.DateTime.Now;
                siniestro.SiniestroEstadoId = (int)SiniestroEstadoEnum.Recibido;

                // Calcular automáticamente el número como el último número + 1
                var ultimoNumero = await _unitOfWork.Siniestros.GetUltimoNumeroAsync(cancellationToken);
                siniestro.Numero = ultimoNumero + 1;

                await _unitOfWork.Siniestros.AddAsync(siniestro, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);

                

                // Registrar historial inicial
                await _unitOfWork.SiniestroEstadoHistoriales.AddAsync(new SiniestroEstadoHistorial
                {
                    SiniestroId = siniestro.Id,
                    SiniestroEstadoId = (int)SiniestroEstadoEnum.Recibido,
                    Fecha = DateTime.UtcNow
                }, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Siniestro registrado exitosamente con ID {SiniestroId} y Número autogenerado {Numero}", siniestro.Id, siniestro.Numero);
                var result = await GetByIdAsync(siniestro.Id, cancellationToken);
                return ServiceResponse<SiniestroDto>.Ok(result.Data!, "Siniestro creado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al crear el siniestro para Empleador {EmpleadorId} y Trabajador {TrabajadorId}", empleador.Id, trabajador.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.SystemError, ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> CambiarEstadoAsync(int siniestroId, int nuevoEstadoId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Solicitud de cambio de estado recibida para Siniestro ID {SiniestroId} al Estado ID {NuevoEstadoId}", siniestroId, nuevoEstadoId);

            // 1. Validar que el nuevo estado exista usando ISiniestroEstadoService
            var estadoValidoRes = await _siniestroEstadoService.ExisteEstadoAsync(nuevoEstadoId, cancellationToken);
            if (!estadoValidoRes.Success)
            {
                _logger.LogWarning("Cambio de estado rechazado: Estado de siniestro con ID {NuevoEstadoId} no existe.", nuevoEstadoId);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.EstadoNoDisponible);
            }

            // 2. Obtener el siniestro por su ID
            var siniestro = await _unitOfWork.Siniestros.GetByIdAsync(siniestroId, cancellationToken);
            if (siniestro == null)
            {
                _logger.LogWarning("Cambio de estado rechazado: Siniestro con ID {SiniestroId} no fue encontrado.", siniestroId);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.SiniestroNotFound);
            }

            var estadoAnteriorId = siniestro.SiniestroEstadoId;
            _logger.LogInformation("Cambiando el estado del siniestro ID {SiniestroId} de {EstadoAnteriorId} a {NuevoEstadoId}", siniestroId, estadoAnteriorId, nuevoEstadoId);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 3. Actualizar estado del siniestro
                siniestro.SiniestroEstadoId = nuevoEstadoId;
                _unitOfWork.Siniestros.Update(siniestro);

                // 4. Registrar en el historial de estados
                var historial = new SiniestroEstadoHistorial
                {
                    SiniestroId = siniestroId,
                    SiniestroEstadoId = nuevoEstadoId,
                    Fecha = DateTime.UtcNow
                };

                await _unitOfWork.SiniestroEstadoHistoriales.AddAsync(historial, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Estado del siniestro ID {SiniestroId} cambiado exitosamente de {EstadoAnteriorId} a {NuevoEstadoId}.", siniestroId, estadoAnteriorId, nuevoEstadoId);

                // 5. Si cambia a Aprobado (SiniestroEstadoId == 3), invocar a IStrNotificationService
                if (nuevoEstadoId == (int)SiniestroEstadoEnum.Aprobado)
                {
                    try
                    {
                        await _strNotificationService.NotificarAprobacionSrtAsync(siniestroId, cancellationToken);
                    }
                    catch (Exception srtEx)
                    {
                        _logger.LogError(srtEx, "⚠️ Error no bloqueante al notificar a la SRT para el Siniestro ID {SiniestroId}", siniestroId);
                    }
                }

                return ServiceResponse<bool>.Ok(true, "Estado de siniestro modificado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al intentar cambiar el estado del siniestro ID {SiniestroId} a {NuevoEstadoId}", siniestroId, nuevoEstadoId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.SystemError, ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> AsignarPrestadorAsync(int siniestroId, int prestadorId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Solicitud de asignación de prestador ID {PrestadorId} al siniestro ID {SiniestroId}", prestadorId, siniestroId);

            // 1. Validar existencia de Siniestro
            var siniestro = await _unitOfWork.Siniestros.GetByIdAsync(siniestroId, cancellationToken);
            if (siniestro == null)
            {
                _logger.LogWarning("Asignación de prestador fallida: Siniestro ID {SiniestroId} no fue encontrado.", siniestroId);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.SiniestroNotFound);
            }

            // 2. Validar existencia de Prestador a través de IPrestadorService
            var prestadorRes = await _prestadorService.GetByIdAsync(prestadorId, cancellationToken);
            if (!prestadorRes.Success || prestadorRes.Data == null)
            {
                _logger.LogWarning("Asignación de prestador fallida: Prestador ID {PrestadorId} no fue encontrado.", prestadorId);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.PrestadorNotFound);
            }

            // 3. Validar si la relación ya se encuentra registrada
            var relacionExiste = await _unitOfWork.SiniestroPrestadores.ExistsAsync(
                sp => sp.SiniestroId == siniestroId && sp.PrestadorId == prestadorId,
                cancellationToken);

            if (relacionExiste)
            {
                _logger.LogWarning("Asignación de prestador fallida: El prestador ID {PrestadorId} ya está asignado al siniestro ID {SiniestroId}.", prestadorId, siniestroId);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.PrestadorYaAsignado);
            }

            try
            {
                // 4. Crear y guardar la relación en Siniestro_Prestador
                await _unitOfWork.SiniestroPrestadores.AddAsync(new Siniestro_Prestador
                {
                    SiniestroId = siniestroId,
                    PrestadorId = prestadorId
                }, cancellationToken);

                await _unitOfWork.CompleteAsync(cancellationToken);

                _logger.LogInformation("Prestador ID {PrestadorId} asignado exitosamente al siniestro ID {SiniestroId}.", prestadorId, siniestroId);
                return ServiceResponse<bool>.Ok(true, "Prestador asignado correctamente al siniestro.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al asignar el prestador ID {PrestadorId} al siniestro ID {SiniestroId}", prestadorId, siniestroId);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.SystemError, ex.Message);
            }
        }

        private async Task LoadPrestadoresYHistorialAsync(SiniestroDto dto, CancellationToken cancellationToken)
        {
            var prestadoresRes = await _prestadorService.GetPrestadoresPorSiniestrosAsync(dto.Id, cancellationToken);
            if (prestadoresRes.Success && prestadoresRes.Data != null)
            {
                dto.Prestadores = prestadoresRes.Data.ToList();
            }

            var historiales = await _unitOfWork.SiniestroEstadoHistoriales.GetHistorialPorSiniestroAsync(dto.Id, cancellationToken);
            dto.HistorialEstados = _mapper.Map<List<SiniestroEstadoHistorialDto>>(historiales);

            var notificacionesRes = await _strNotificationService.GetBySiniestroIdAsync(dto.Id, cancellationToken);
            if (notificacionesRes.Success && notificacionesRes.Data != null)
            {
                dto.NotificacionesSRT = notificacionesRes.Data.ToList();
            }
        }
    }
}

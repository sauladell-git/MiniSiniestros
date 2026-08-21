using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Enums;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Str;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class StrNotificationService : IStrNotificationService
    {
        private readonly IUoWData _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<StrNotificationService> _logger;
        private readonly ISrtNotificationClient _srtClient;

        public StrNotificationService(
            IUoWData unitOfWork,
            IMapper mapper,
            ILogger<StrNotificationService> logger,
            ISrtNotificationClient srtClient)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _srtClient = srtClient ?? throw new ArgumentNullException(nameof(srtClient));
        }

        public async Task<ServiceResponse<NotificacionSrtDto>> NotificarAprobacionSrtAsync(int siniestroId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("📢 [SRT-SERVICE] Iniciando proceso de notificación SRT para Siniestro ID {SiniestroId}", siniestroId);

            var siniestro = await _unitOfWork.Siniestros.GetByIdAsync(siniestroId, cancellationToken);
            if (siniestro == null)
            {
                _logger.LogWarning("⚠️ [SRT-SERVICE] Siniestro con ID {SiniestroId} no encontrado.", siniestroId);
                return ServiceResponse<NotificacionSrtDto>.Fail(SiniestroErrorConstants.SiniestroNotFound);
            }

            var payload = new SrtPayloadDto
            {
                SiniestroId = siniestroId,
                FechaAprobacion = DateTime.UtcNow,
                Estado = SiniestroEstadoEnum.Aprobado.ToString()
            };

            var outcome = await _srtClient.NotificarAprobacionAsync(payload, cancellationToken);

            var notificacionSRT = new NotificacionSRT
            {
                SiniestroId = siniestroId,
                Timestamp = DateTime.UtcNow,
                Status = outcome.Status,
                Payload = JsonSerializer.Serialize(payload),
                Intentos = outcome.Intentos > 0 ? outcome.Intentos : 1
            };

            await _unitOfWork.NotificacionesSRT.AddAsync(notificacionSRT, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            _logger.LogInformation(" [SRT-SERVICE] Notificación a la SRT registrada en DB con ID {NotificacionId}. Status: {Status}, Intentos: {Intentos}",
                notificacionSRT.Id, outcome.Status, outcome.Intentos);

            var dto = _mapper.Map<NotificacionSrtDto>(notificacionSRT);
            return ServiceResponse<NotificacionSrtDto>.Ok(dto);
        }

        public async Task<ServiceResponse<IReadOnlyList<NotificacionSrtDto>>> GetBySiniestroIdAsync(int siniestroId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando Notificaciones SRT para el Siniestro ID {SiniestroId}", siniestroId);

            var notificaciones = await _unitOfWork.NotificacionesSRT.GetAsync(n => n.SiniestroId == siniestroId, cancellationToken: cancellationToken);
            var dtos = _mapper.Map<IReadOnlyList<NotificacionSrtDto>>(notificaciones);

            return ServiceResponse<IReadOnlyList<NotificacionSrtDto>>.Ok(dtos);
        }
    }
}

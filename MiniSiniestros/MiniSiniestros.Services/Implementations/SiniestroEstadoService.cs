using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class SiniestroEstadoService : ISiniestroEstadoService
    {
        private readonly IUoWData _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SiniestroEstadoService> _logger;

        public SiniestroEstadoService(
            IUoWData unitOfWork,
            IMapper mapper,
            ILogger<SiniestroEstadoService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<SiniestroEstadoDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando estado de siniestro con ID {EstadoId}", id);

            var estado = await _unitOfWork.SiniestroEstados.GetByIdAsync(id, cancellationToken);
            if (estado == null)
            {
                _logger.LogWarning("Estado de siniestro con ID {EstadoId} no fue encontrado.", id);
                return ServiceResponse<SiniestroEstadoDto>.Fail(SiniestroErrorConstants.EstadoNoDisponible);
            }

            var dto = _mapper.Map<SiniestroEstadoDto>(estado);
            return ServiceResponse<SiniestroEstadoDto>.Ok(dto);
        }

        public async Task<ServiceResponse<IReadOnlyList<SiniestroEstadoDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando todos los estados de siniestro.");

            var estados = await _unitOfWork.SiniestroEstados.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IReadOnlyList<SiniestroEstadoDto>>(estados);
            return ServiceResponse<IReadOnlyList<SiniestroEstadoDto>>.Ok(dtos);
        }

        public async Task<ServiceResponse<bool>> ExisteEstadoAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Verificando existencia del estado de siniestro con ID {EstadoId}", id);

            var existe = await _unitOfWork.SiniestroEstados.ExistsAsync(e => e.Id == id, cancellationToken);
            if (!existe)
            {
                _logger.LogWarning("Validación de existencia fallida: Estado de siniestro con ID {EstadoId} no existe.", id);
                return ServiceResponse<bool>.Fail(SiniestroErrorConstants.EstadoNoDisponible);
            }

            return ServiceResponse<bool>.Ok(true);
        }
    }
}

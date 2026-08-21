using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Prestador;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class PrestadorService : IPrestadorService
    {
        private readonly IUoWData _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PrestadorService> _logger;

        public PrestadorService(IUoWData unitOfWork, IMapper mapper, ILogger<PrestadorService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<IReadOnlyList<PrestadorDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando el catálogo completo de prestadores.");
            var prestadores = await _unitOfWork.Prestadores.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IReadOnlyList<PrestadorDto>>(prestadores);
            return ServiceResponse<IReadOnlyList<PrestadorDto>>.Ok(dtos, "Prestadores obtenidos correctamente.");
        }

        public async Task<ServiceResponse<PrestadorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando prestador con ID {PrestadorId}", id);

            var prestador = await _unitOfWork.Prestadores.GetByIdAsync(id, cancellationToken);
            if (prestador == null)
            {
                _logger.LogWarning("Prestador con ID {PrestadorId} no fue encontrado.", id);
                return ServiceResponse<PrestadorDto>.Fail(PrestadorErrorConstants.PrestadorNotFound);
            }

            var dto = _mapper.Map<PrestadorDto>(prestador);
            return ServiceResponse<PrestadorDto>.Ok(dto);
        }

        public async Task<ServiceResponse<IReadOnlyList<PrestadorDto>>> GetPrestadoresPorSiniestrosAsync(int siniestroId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando prestadores asignados al Siniestro ID {SiniestroId}", siniestroId);

            var prestadoresAsignados = await _unitOfWork.SiniestroPrestadores.GetPrestadoresPorSiniestroAsync(siniestroId, cancellationToken);
            var dtos = prestadoresAsignados
                .Where(sp => sp.Prestador != null)
                .Select(sp => _mapper.Map<PrestadorDto>(sp.Prestador))
                .ToList();

            return ServiceResponse<IReadOnlyList<PrestadorDto>>.Ok(dtos);
        }
    }
}

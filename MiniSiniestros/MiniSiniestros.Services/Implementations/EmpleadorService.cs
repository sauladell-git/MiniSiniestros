using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Empleador;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class EmpleadorService : IEmpleadorService
    {
        private readonly IUoWData _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmpleadorService> _logger;

        public EmpleadorService(IUoWData unitOfWork, IMapper mapper, ILogger<EmpleadorService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<EmpleadorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando empleador con ID {EmpleadorId}", id);

            var empleador = await _unitOfWork.Empleadores.GetByIdAsync(id, cancellationToken);
            if (empleador == null)
            {
                _logger.LogWarning("Empleador con ID {EmpleadorId} no fue encontrado.", id);
                return ServiceResponse<EmpleadorDto>.Fail(EmpleadorErrorConstants.EmpleadorNotFound);
            }

            var dto = _mapper.Map<EmpleadorDto>(empleador);
            return ServiceResponse<EmpleadorDto>.Ok(dto);
        }

        public async Task<ServiceResponse<EmpleadorDto>> GetByCuitAsync(string cuit, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando empleador con CUIT {Cuit}", cuit);

            var empleador = await _unitOfWork.Empleadores.GetByCuitAsync(cuit, cancellationToken);
            if (empleador == null)
            {
                _logger.LogWarning("Empleador con CUIT {Cuit} no fue encontrado.", cuit);
                return ServiceResponse<EmpleadorDto>.Fail(EmpleadorErrorConstants.EmpleadorNotFound);
            }

            var dto = _mapper.Map<EmpleadorDto>(empleador);
            return ServiceResponse<EmpleadorDto>.Ok(dto);
        }
    }
}

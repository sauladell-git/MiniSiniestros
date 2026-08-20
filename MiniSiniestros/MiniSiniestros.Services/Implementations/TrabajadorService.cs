using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Trabajador;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class TrabajadorService : ITrabajadorService
    {
        private readonly IUoWData _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TrabajadorService> _logger;

        public TrabajadorService(IUoWData unitOfWork, IMapper mapper, ILogger<TrabajadorService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<TrabajadorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando trabajador con ID {TrabajadorId}", id);

            var trabajadores = await _unitOfWork.Trabajadores.GetAsync(
                predicate: t => t.Id == id,
                includeProperties: "Empleador",
                cancellationToken: cancellationToken);

            var trabajador = trabajadores.FirstOrDefault();
            if (trabajador == null)
            {
                _logger.LogWarning("Trabajador con ID {TrabajadorId} no fue encontrado.", id);
                return ServiceResponse<TrabajadorDto>.Fail(TrabajadorErrorConstants.TrabajadorNotFound);
            }

            var dto = _mapper.Map<TrabajadorDto>(trabajador);
            return ServiceResponse<TrabajadorDto>.Ok(dto);
        }

        public async Task<ServiceResponse<TrabajadorDto>> GetByCuilAsync(string cuil, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando trabajador con CUIL {Cuil}", cuil);

            var trabajador = await _unitOfWork.Trabajadores.GetByCuilAsync(cuil, cancellationToken);
            if (trabajador == null)
            {
                _logger.LogWarning("Trabajador con CUIL {Cuil} no fue encontrado.", cuil);
                return ServiceResponse<TrabajadorDto>.Fail(TrabajadorErrorConstants.TrabajadorNotFound);
            }

            var dto = _mapper.Map<TrabajadorDto>(trabajador);
            return ServiceResponse<TrabajadorDto>.Ok(dto);
        }

        public async Task<ServiceResponse<bool>> ExistePorTrabajadorYEmpleadorAsync(int trabajadorId, int empleadorId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Verificando existencia de trabajador ID {TrabajadorId} asociado al empleador ID {EmpleadorId}", trabajadorId, empleadorId);

            var existe = await _unitOfWork.Trabajadores.ExistsAsync(
                t => t.Id == trabajadorId && t.EmpleadorId == empleadorId,
                cancellationToken);

            if (!existe)
            {
                _logger.LogWarning("Trabajador ID {TrabajadorId} no existe o no pertenece al empleador ID {EmpleadorId}", trabajadorId, empleadorId);
            }

            return ServiceResponse<bool>.Ok(existe);
        }
    }
}

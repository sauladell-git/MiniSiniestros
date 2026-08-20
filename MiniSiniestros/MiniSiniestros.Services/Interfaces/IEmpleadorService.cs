using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Empleador;

namespace MiniSiniestros.Services.Interfaces
{
    public interface IEmpleadorService
    {
        Task<ServiceResponse<EmpleadorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<EmpleadorDto>> GetByCuitAsync(string cuit, CancellationToken cancellationToken = default);
    }
}

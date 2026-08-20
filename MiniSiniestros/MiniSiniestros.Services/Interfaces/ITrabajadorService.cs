using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Trabajador;

namespace MiniSiniestros.Services.Interfaces
{
    public interface ITrabajadorService
    {
        Task<ServiceResponse<TrabajadorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<TrabajadorDto>> GetByCuilAsync(string cuil, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> ExistePorTrabajadorYEmpleadorAsync(int trabajadorId, int empleadorId, CancellationToken cancellationToken = default);
    }
}

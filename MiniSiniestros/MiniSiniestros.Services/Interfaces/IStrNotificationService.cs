using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Str;

namespace MiniSiniestros.Services.Interfaces
{
    public interface IStrNotificationService
    {
        Task<ServiceResponse<NotificacionSrtDto>> NotificarAprobacionSrtAsync(int siniestroId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IReadOnlyList<NotificacionSrtDto>>> GetBySiniestroIdAsync(int siniestroId, CancellationToken cancellationToken = default);
    }
}

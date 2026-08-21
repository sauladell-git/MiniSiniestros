using MiniSiniestros.Dto.Str;

namespace MiniSiniestros.Services.Interfaces
{
    public interface ISrtNotificationClient
    {
        Task<SrtNotificationOutcomeDto> NotificarAprobacionAsync(SrtPayloadDto payload, CancellationToken cancellationToken = default);
    }
}

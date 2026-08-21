using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class NotificacionSRTRepository : Repository<NotificacionSRT>, INotificacionSRTRepository
    {
        public NotificacionSRTRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }
    }
}

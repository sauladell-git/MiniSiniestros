using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiniestrosController : ControllerBase
    {
        private readonly ISiniestroService _siniestroService;

        public SiniestrosController(ISiniestroService siniestroService)
        {
            _siniestroService = siniestroService ?? throw new ArgumentNullException(nameof(siniestroService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<IReadOnlyList<SiniestroDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ServiceResponse<IReadOnlyList<SiniestroDto>>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _siniestroService.GetAllAsync(cancellationToken);
            return Ok(response);
        }
    }
}

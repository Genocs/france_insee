using Microsoft.AspNetCore.Mvc;
using Genocs.FranceInsee.Infrastructure.ApiClients;

namespace Genocs.FranceInsee.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ISireneApiClient apiClient;

        public HomeController(ILogger<HomeController> logger, ISireneApiClient apiClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        [HttpGet("Siren")]
        public async Task<IActionResult> GetSiren(string siret)
        {
            object result = await this.apiClient.GetSirenAsync(siret, await InternalGetToken());
            return Ok(result);
        }

        [HttpGet("Siret")]
        public async Task<IActionResult> GetSiret(string siren)
        {
            object result = await this.apiClient.GetSiretAsync(siren, await InternalGetToken());
            return Ok(result);
        }

        [HttpGet("Token")]
        public async Task<IActionResult> GetToken()
        {
            return Ok(await InternalGetToken());
        }

        private async Task<string> InternalGetToken()
        {
            Dictionary<string, object> token = new Dictionary<string, object>();
            token.Add("grant_type", "client_credentials");
            var res = await this.apiClient.PostTokenAsync(token);
            return $"Bearer {res.access_token}";
        }
    }
}
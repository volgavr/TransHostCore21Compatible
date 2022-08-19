using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TransHostService.Helpers;

namespace TransHostCore21Compatible.Controllers
{
    /// <inheritdoc />    
    [Route("api/[controller]")]
    [ApiController]
    public class HostsController : ControllerBase
    {
        private readonly ILogger<HostsController> _logger;
        private readonly IConfiguration _config;

        public HostsController(ILogger<HostsController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("distance")]
        public async Task<ActionResult<double>> GetAsync(string fromHub, string toHub)
        {
            try
            {
                if (_config["TeleportUrl"] == null)
                    throw new InvalidOperationException("Configuration option [TeleportUrl] is not defined");

                var requestUri = new Uri(_config["TeleportUrl"]);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = requestUri;
                    var fromCoord = JsonHelper.GetAirportCoordinatesAsync(client, fromHub);
                    var toCoord = JsonHelper.GetAirportCoordinatesAsync(client, toHub);
                    await Task.WhenAll(fromCoord, toCoord);
                    return GeoPoint.DistanceBetween(fromCoord.Result, toCoord.Result);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Exception thrown");
                return BadRequest();
            }

        }
    }
}

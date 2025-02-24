using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BespokedBikesAPI.Controllers
{
    /// <summary>
    /// Status
    /// </summary>
    /// <remarks>
    /// A simple endpoint used to check the status of the endpoint.
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        #region GETs

        /// <summary>
        /// Get Status
        /// </summary>
        /// <remarks>
        /// Gets the status of the endpoint
        /// </remarks>
        /// <returns>The current status of the endpoint. Includes: application name, the hostname the application is running on, the uptime, and the application version.</returns>
        [HttpGet(Name = "GetStatus")]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            return Ok(new
            {
                ok = true,
                app_name = Assembly.GetExecutingAssembly().GetName().Name,
#if !RELEASE
                build_config = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration,
#endif
                hostname = System.Net.Dns.GetHostName(),
                uptime = (DateTime.Now - (DateTimeOffset)System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()).TotalSeconds,
                version = Assembly.GetExecutingAssembly().GetName().Version?.Major
            });
        }

        #endregion
    }
}

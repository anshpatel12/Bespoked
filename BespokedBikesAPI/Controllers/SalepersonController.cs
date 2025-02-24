using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace BespokedBikesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalepersonController : ControllerBase
    {
        private readonly ILogger<SalepersonController> _logger;
        private readonly IConfiguration _config;

        public SalepersonController(ILogger<SalepersonController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Get Saleperson
        /// </summary>
        /// <remarks>
        /// A function for retrieving information on all saleperson
        /// </remarks>
        /// <returns>A list of Saleperson</returns>
        [HttpGet(Name = "GetSaleperson")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetSaleperson()
        {
            try
            {
                _logger.LogInformation("Begin GetSaleperson Endpoint");

                List<Saleperson> salepersonList = new();

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string query = "SELECT \"SalespersonId\", \"FirstName\", \"LastName\", \"Address\", \"Phone\", \"StartDate\", \"TerminationDate\", \"Manager\" FROM \"Salesperson\"";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using var cmd = new NpgsqlCommand(query, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Saleperson objSaleperson = new()
                            {
                                SalepersonId = reader.GetInt64(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Address = reader.GetString(3),
                                Phone = reader.GetString(4),
                                StartDate = reader.GetDateTime(5),
                                TerminationDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                Manager = reader.GetString(7)
                            };

                            salepersonList.Add(objSaleperson);
                        }
                    }
                }

                return Ok(new ResponseModel(true, salepersonList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseModel(false, false, $"Exception Type :- {ex.GetType()} | Message :- {ex.Message} | InnerException :- {ex.InnerException} | StackTrace :- {ex.StackTrace}"));
            }
        }

        [HttpPut("{id}", Name = "UpdateSalesperson")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateSalesperson(long id, [FromBody] Saleperson salesperson)
        {
            try
            {
                _logger.LogInformation($"Updating salesperson {id}");

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string query = "UPDATE \"Salesperson\" SET \"FirstName\" = @firstName, \"LastName\" = @lastName, \"Address\" = @address, \"Phone\" = @phone, \"StartDate\" = @startDate, \"TerminationDate\" = @terminationDate, \"Manager\" = @manager WHERE \"SalespersonId\" = @id";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("firstName", salesperson.FirstName);
                    cmd.Parameters.AddWithValue("lastName", salesperson.LastName);
                    cmd.Parameters.AddWithValue("address", salesperson.Address);
                    cmd.Parameters.AddWithValue("phone", salesperson.Phone);
                    cmd.Parameters.AddWithValue("startDate", salesperson.StartDate);
                    cmd.Parameters.AddWithValue("terminationDate", salesperson.TerminationDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("manager", salesperson.Manager);

                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    if (affectedRows > 0)
                        return Ok(new ResponseModel(true, "Salesperson updated successfully"));
                    else
                        return NotFound(new ResponseModel(false, "Salesperson not found"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseModel(false, $"Error: {ex.Message}"));
            }
        }

    }
}

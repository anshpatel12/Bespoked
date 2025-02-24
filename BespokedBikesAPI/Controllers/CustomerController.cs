using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace BespokedBikesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly IConfiguration _config;

        public CustomerController(ILogger<CustomerController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Get Customers
        /// </summary>
        /// <remarks>
        /// A function for retrieving information on all customers
        /// </remarks>
        /// <returns>A list of Customers</returns>
        [HttpGet(Name = "GetCustomers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetCustomers()
        {
            try
            {
                _logger.LogInformation("Begin GetCustomers Endpoint");

                List<Customer> customerList = new();

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string query = "SELECT \"CustomerId\", \"FirstName\", \"LastName\", \"Address\", \"Phone\", \"StartDate\" FROM \"Customer\"";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using var cmd = new NpgsqlCommand(query, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Customer objCustomer = new()
                            {
                                CustomerId = reader.GetInt64(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Address = reader.GetString(3),
                                Phone = reader.GetString(4),
                                StartDate = reader.GetDateTime(5),
                            };

                            customerList.Add(objCustomer);
                        }
                    }
                }

                return Ok(new ResponseModel(true, customerList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseModel(false, false, $"Exception Type :- {ex.GetType()} | Message :- {ex.Message} | InnerException :- {ex.InnerException} | StackTrace :- {ex.StackTrace}"));
            }
        }
    }
}

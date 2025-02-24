using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace BespokedBikesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IConfiguration _config;

        public ReportController(ILogger<ReportController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("QuarterlyCommission", Name = "GetQuarterlyCommissionReport")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetQuarterlyCommissionReport([FromQuery] int year, [FromQuery] int quarter)
        {
            try
            {
                _logger.LogInformation($"Generating quarterly commission report for {year} Q{quarter}");

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string query = @"
                    SELECT 
                        sp.""FirstName"", sp.""LastName"", 
                        SUM(p.""SalePrice"" * p.""CommissionPct"" / 100 * s.""QuantitySold"") AS total_commission 
                    FROM ""Sales"" s
                    JOIN ""Products"" p ON s.""ProductId"" = p.""ProductId""
                    JOIN ""Salesperson"" sp ON s.""SalespersonId"" = sp.""SalespersonId""
                    WHERE EXTRACT(YEAR FROM s.""SaleDate"") = @year
                        AND EXTRACT(QUARTER FROM s.""SaleDate"") = @quarter
                    GROUP BY sp.""FirstName"", sp.""LastName"";";

                List<object> reportData = new();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("year", year);
                    cmd.Parameters.AddWithValue("quarter", quarter);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reportData.Add(new
                            {
                                Salesperson = $"{reader.GetString(0)} {reader.GetString(1)}",
                                TotalCommission = reader.GetDouble(2)
                            });
                        }
                    }
                }

                return Ok(new ResponseModel(true, reportData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseModel(false, $"Error: {ex.Message}"));
            }
        }
    }
}

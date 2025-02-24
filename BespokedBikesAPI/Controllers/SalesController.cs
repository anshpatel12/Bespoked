using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace BespokedBikesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly IConfiguration _config;

        public SalesController(ILogger<SalesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Get Sales
        /// </summary>
        /// <remarks>
        /// A function for retrieving information on all sales
        /// </remarks>
        /// <param name="startDate">Optional Parameter Start Date </param>
        /// <param name="endDate">Optional Parameter End Date</param>
        /// <returns>A list of Sales</returns>
        [HttpGet(Name = "GetSales")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetSales([FromQuery] DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Begin GetSales Endpoint");


                string whereClause = "";

                if (startDate != null && endDate != null)
                {
                    whereClause = $" WHERE \"SaleDate\" >= '{startDate}' AND \"SaleDate\" <= '{endDate}'";
                }
                else if (startDate != null && endDate == null)
                {
                    whereClause = $" WHERE \"SaleDate\" >= '{startDate}'";
                }
                else if (startDate == null && endDate != null)
                {
                    whereClause = $" WHERE \"SaleDate\" <= '{endDate}'";
                }
                else
                    whereClause = "";

                List<TotalSales> salesList = new();

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string query = $"SELECT \"SaleId\", productname, customername, \"SaleDate\", \"QuantitySold\", totalsaleprice, salespersonname, salespersoncommission FROM public.\"vw_TotalSales\" {whereClause}";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using var cmd = new NpgsqlCommand(query, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            TotalSales objSales = new()
                            {
                                SaleId = reader.GetInt64(0),
                                ProductName = reader.GetString(1),
                                CustomerName = reader.GetString(2),
                                SaleDate = reader.GetDateTime(3),
                                QuantitySold = reader.GetInt32(4),
                                TotalSalePrice = reader.GetDouble(5),
                                SalesPersonName = reader.GetString(6),
                                SalesPersonCommission = reader.GetDouble(7)
                            };

                            salesList.Add(objSales);
                        }
                    }
                }

                return Ok(new ResponseModel(true, salesList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseModel(false, false, $"Exception Type :- {ex.GetType()} | Message :- {ex.Message} | InnerException :- {ex.InnerException} | StackTrace :- {ex.StackTrace}"));
            }

        }

        /// <summary>
        /// Create Sale
        /// </summary>
        /// <remarks>
        /// A function for creating sales
        /// </remarks>
        /// <param name="objSale">See Class Defination</param>
        /// <returns>New Sale Id</returns>
        [HttpPost(Name = "CreateSale")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateSale([FromBody] Sale objSale)
        {
            try
            {
                _logger.LogInformation("Begin CreateSale Endpoint");

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string getQuantityQuery = "SELECT \"QtyOnHand\" FROM \"Products\" WHERE  \"ProductId\" = @productid";

                long qtyOnHand = 0;

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using var cmd = new NpgsqlCommand(getQuantityQuery, conn);

                    cmd.Parameters.AddWithValue("productid", objSale.ProductId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            qtyOnHand = reader.GetInt64(0);

                        }
                    }
                }

                if (qtyOnHand >= objSale.QuantitySold)
                {

                    string query = "INSERT INTO public.\"Sales\"(\"ProductId\", \"SalespersonId\", \"CustomerId\", \"SaleDate\", \"QuantitySold\") VALUES (@productid, @salespersonid, @customerid, @saledate, @quantitysold) RETURNING \"SaleId\";";

                    long newSaleId = 0;

                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();

                        using (var cmd = new NpgsqlCommand(query, conn))
                        {

                            cmd.Parameters.AddWithValue("productid", objSale.ProductId);
                            cmd.Parameters.AddWithValue("salespersonid", objSale.SalespersonId);
                            cmd.Parameters.AddWithValue("customerid", objSale.CustomerId);
                            cmd.Parameters.AddWithValue("saledate", objSale.SaleDate);
                            cmd.Parameters.AddWithValue("quantitysold", objSale.QuantitySold);

                            newSaleId = (long)await cmd.ExecuteScalarAsync();
                        }

                        if (newSaleId > 0)
                        {

                            string updateQuantityQuery = "UPDATE \"Products\" SET \"QtyOnHand\" = @qtyOnHand WHERE \"ProductId\" = @productid";

                            using (var cmd = new NpgsqlCommand(updateQuantityQuery, conn))
                            {

                                cmd.Parameters.AddWithValue("qtyOnHand", qtyOnHand - objSale.QuantitySold);
                                cmd.Parameters.AddWithValue("productid", objSale.ProductId);

                                await cmd.ExecuteScalarAsync();
                            }
                        }
                    }

                    return Ok(new ResponseModel(true, newSaleId));
                }
                else
                    return BadRequest(new ResponseModel(false, null, "Not Enough Quantity in Stock"));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseModel(false, false, $"Exception Type :- {ex.GetType()} | Message :- {ex.Message} | InnerException :- {ex.InnerException} | StackTrace :- {ex.StackTrace}"));
            }

        }

    }
}
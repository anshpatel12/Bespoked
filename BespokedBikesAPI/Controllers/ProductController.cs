using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace BespokedBikesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IConfiguration _config;

        public ProductController(ILogger<ProductController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Get Products
        /// </summary>
        /// <remarks>
        /// A function for retrieving information on all products
        /// </remarks>
        /// <returns>A list of Products</returns>
        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetProducts()
        {
            try
            {
                _logger.LogInformation("Begin GetProducts Endpoint");

                List<Product> productList = new();

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string query = "SELECT \"ProductId\", \"Name\", \"Manufacturer\", \"Style\", \"PurchasePrice\", \"SalePrice\", \"QtyOnHand\", \"CommissionPct\" FROM \"Products\" WHERE \"IsDeleted\" = false";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using var cmd = new NpgsqlCommand(query, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Product objProduct = new()
                            {
                                ProductId = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Manufacturer = reader.GetString(2),
                                Style = reader.GetString(3),
                                PurchasePrice = reader.GetDouble(4),
                                SalePrice = reader.GetDouble(5),
                                QtyOnHand = reader.GetInt64(6),
                                CommissionPct = reader.GetFloat(7),
                            };

                            productList.Add(objProduct);
                        }
                    }
                }

                return Ok(new ResponseModel(true, productList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseModel(false, false, $"Exception Type :- {ex.GetType()} | Message :- {ex.Message} | InnerException :- {ex.InnerException} | StackTrace :- {ex.StackTrace}"));
            }
        }

        [HttpPut("{id}", Name = "UpdateProduct")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateProduct(long id, [FromBody] Product product)
        {
            try
            {
                _logger.LogInformation($"Updating product {id}");

                var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection");

                string query = "UPDATE \"Products\" SET \"Name\" = @name, \"Manufacturer\" = @manufacturer, \"Style\" = @style, \"PurchasePrice\" = @purchasePrice, \"SalePrice\" = @salePrice, \"QtyOnHand\" = @qtyOnHand, \"CommissionPct\" = @commissionPct WHERE \"ProductId\" = @id";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("name", product.Name);
                    cmd.Parameters.AddWithValue("manufacturer", product.Manufacturer);
                    cmd.Parameters.AddWithValue("style", product.Style);
                    cmd.Parameters.AddWithValue("purchasePrice", product.PurchasePrice);
                    cmd.Parameters.AddWithValue("salePrice", product.SalePrice);
                    cmd.Parameters.AddWithValue("qtyOnHand", product.QtyOnHand);
                    cmd.Parameters.AddWithValue("commissionPct", product.CommissionPct);

                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    if (affectedRows > 0)
                        return Ok(new ResponseModel(true, "Product updated successfully"));
                    else
                        return NotFound(new ResponseModel(false, "Product not found"));
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

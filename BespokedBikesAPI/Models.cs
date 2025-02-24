namespace BespokedBikesAPI
{
    public class Product
    {
        public long ProductId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Style { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public long QtyOnHand { get; set; }
        public float CommissionPct { get; set; }
    }

    public class Saleperson
    {
        public long SalepersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string Manager { get; set; }
    }

    public class Customer
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class TotalSales
    {
        public long SaleId { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
        public int QuantitySold { get; set; }
        public double TotalSalePrice { get; set; }
        public string SalesPersonName { get; set; }
        public double SalesPersonCommission { get; set; }
    }

    public class Sale
    {
        public long SaleId { get; set; }
        public long ProductId { get; set; }
        public long SalespersonId { get; set; }
        public long CustomerId { get; set; }
        public int QuantitySold { get; set; }
        public DateTime SaleDate { get; set; }

    }


    /// <summary>
    /// The default response model used for Database Connector controllers
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// The success state of the request
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The data, if any, included in the response
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Any stateful message that the system can use for debugging
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The default class constructor
        /// </summary>
        public ResponseModel()
        {
            Data = null;
            Message = "";
            IsSuccess = false;
        }

        /// <summary>
        /// A simple constructor with no data or message
        /// </summary>
        /// <param name="isSuccess">The success state of the request</param>
        public ResponseModel(bool isSuccess)
        {
            IsSuccess = isSuccess;
            Message = "";
        }

        /// <summary>
        /// A simple constructor with no message
        /// </summary>
        /// <param name="isSuccess">The success state of the request</param>
        /// <param name="data">The data, if any, to include in the response</param>
        public ResponseModel(bool isSuccess, object? data)
        {
            Message = "";
            IsSuccess = isSuccess;
            Data = data;
        }

        /// <summary>
        /// A simple constructor with no message
        /// </summary>
        /// <param name="isSuccess">The success state of the request</param>
        /// <param name="data">The data, if any, to include in the response</param>
        /// <param name="message">Any stateful message that the system can use for debugging</param>
        public ResponseModel(bool isSuccess, object? data, string message)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }
    }
}

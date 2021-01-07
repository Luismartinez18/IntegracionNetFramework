using IntegrationWS.Data;
using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegrationWS.Controllers
{
    [Authorize]
    [RoutePrefix("api/price")]
    public class PriceController : ApiController
    {
        private readonly DevelopmentDbContext db = new DevelopmentDbContext();
        [HttpPost]
        [Route("getPrices")]
        public IHttpActionResult GetPrices([FromBody] GetPricesRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var prices = db.Database.SqlQuery<Price>("ListaPreciosPorClientePorProducto @customer,@product,@currency",
            new SqlParameter("@product", request.Product),
            new SqlParameter("@customer", request.Customer),
            new SqlParameter("@currency", request.CurrencyIsoCode));
            var gpr = new GetPriceResponse();
            gpr.Product = request.Product;
            gpr.Prices = prices.Select(x => new GetPriceResponse.Price { UnitOfMeasure = x.UnitOfMeasure, UnitPrice = x.UnitPrice }).ToList();

            return Ok(gpr);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }
        public class GetPricesRequest
        {
            [Required]
            public string Product { get; set; }
            [Required]
            public string Customer { get; set; }
            [Required]
            public string CurrencyIsoCode { get; set; }
        }
        public class GetPriceResponse
        {
            public string Product { get; set; }
            public List<Price> Prices { get; set; }
            public class Price
            {
                public string UnitOfMeasure { get; set; }
                public decimal UnitPrice { get; set; }
            }
        }
    }
}

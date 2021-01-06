using IntegrationWS.Data;
using System;
using System.Collections.Generic;
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
        [HttpGet]
        [Route("getuofm")]
        public IHttpActionResult GetUOFM([FromUri]string customer, [FromUri]string product, [FromUri]string currency)
        {
            if (string.IsNullOrEmpty(customer) ||
                string.IsNullOrEmpty(product) ||
                string.IsNullOrEmpty(currency))
                return BadRequest();
            var uofms = db.Database.SqlQuery<string>("Select QuantityUnitOfMeasure FROM Prices Where Product = @product And Customer = @customer And CurrencyIsoCode = @currency",
                new SqlParameter("@product", product),
                new SqlParameter("@customer", customer),
                new SqlParameter("@currency", currency));
            return Ok(uofms.ToList());
        }
        [HttpGet]
        [Route("getPrice")]
        public IHttpActionResult GetPrice([FromUri]string customer, [FromUri]string product, [FromUri]string currency, [FromUri]string uofm)
        {
            if (string.IsNullOrEmpty(customer) ||
                string.IsNullOrEmpty(product) ||
                string.IsNullOrEmpty(currency) ||
                string.IsNullOrEmpty(uofm))
                return BadRequest();
            var price = db.Database.SqlQuery<decimal?>("Select UnitPrice FROM Prices Where Product = @product And Customer = @customer And CurrencyIsoCode = @currency And QuantityUnitOfMeasure = @uofm",
                new SqlParameter("@product", product),
                new SqlParameter("@customer", customer),
                new SqlParameter("@currency", currency),
                new SqlParameter("@uofm", uofm)).FirstOrDefault();
            if (price == null)
                return NotFound();

            return Ok(price);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }
    }
}

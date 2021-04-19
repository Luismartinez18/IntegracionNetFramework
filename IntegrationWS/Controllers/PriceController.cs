using IntegrationWS.Data;
using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        [HttpPost]
        [Route("getAllPrices")]
        public IHttpActionResult GetAllPrices([FromBody] List<GetPricesRequest> request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest();
            GetPriceResponse[] response = new GetPriceResponse[request.Count];
            Exception e = default;
            Parallel.ForEach(request, (item,state,index) =>
            {
                try
                {
                    using (var tdb = new DevelopmentDbContext())
                    {
                        var prices = tdb.Database.SqlQuery<Price>("ListaPreciosPorClientePorProducto @customer,@product,@currency",
                            new SqlParameter("@product", item.Product),
                            new SqlParameter("@customer", item.Customer),
                            new SqlParameter("@currency", item.CurrencyIsoCode));
                        var gpr = new GetPriceResponse
                        {
                            Product = item.Product,
                            Prices = prices.Select(x => new GetPriceResponse.Price { UnitOfMeasure = x.UnitOfMeasure, UnitPrice = x.UnitPrice }).ToList()
                        };
                        response[Convert.ToInt32(index)] = gpr;
                    }
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            });
            while(true)
            {
                if (request.Exists(x=>x == null) && e == null)
                    continue;
                break;
            }
            if (e != null)
                throw e;
            return Ok(response);
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

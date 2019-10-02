using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegrationWS.Controllers
{
    public class TestController : ApiController
    {
        private readonly ITransferenciasProductos _transferenciasProductos;
        private readonly IAuthToSalesforce _authToSalesforce;

        public TestController(ITransferenciasProductos transferenciasProductos, IAuthToSalesforce authToSalesforce)
        {
            _transferenciasProductos = transferenciasProductos;
            _authToSalesforce = authToSalesforce;
        }

        [HttpGet]
        public IHttpActionResult run()
        {
            return Ok("Bienvenido!");
        }
    }
}

using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IntegrationWS.Controllers
{
    public class BulkApiController : ApiController
    {
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<Asset1> _sobjectCRUD;
        string loginResult = string.Empty;
        public BulkApiController(IAuthToSalesforce authToSalesforce, ISobjectCRUD<Asset1> sobjectCRUD)
        {
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
        }

        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            await Test();
            return Ok();
        }

        private async Task Test()
        {
            try
            {
                loginResult = await _authToSalesforce.Login();
                var algo = await _sobjectCRUD.BulkApi(loginResult, "test", "test");
            }
            catch(Exception e)
            {

            }
        }
    }
}

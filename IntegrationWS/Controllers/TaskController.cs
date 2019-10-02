using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Mail;
using System.Text;
using IntegrationWS.Data;
using IntegrationWS.Models;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;

namespace IntegrationWS.Controllers
{
    [RoutePrefix("api/task")]
    [Authorize]
    public class TaskController : ApiController
    {
        private readonly IReadGpTables _readGpTables;

        public TaskController(IReadGpTables readGpTables)
        {
            _readGpTables = readGpTables;
        }

        [HttpGet]
        [Route("run")]
        public IHttpActionResult run()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Database.ExecuteSqlCommand("UPDATE AppStates SET [State] = 1 WHERE Id = 1");
            }
            _readGpTables.Run();
            return Ok("Iniciado");
        }

        [HttpGet]
        [Route("stop")]
        public IHttpActionResult stop()
        {
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Database.ExecuteSqlCommand("UPDATE AppStates SET [State] = 0 WHERE Id = 1");
            }
            return Ok("Detenido");
        }       
    }
}

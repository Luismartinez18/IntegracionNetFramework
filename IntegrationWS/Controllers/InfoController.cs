using IntegrationWS.Data;
using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegrationWS.Controllers
{
    [RoutePrefix("api/info")]
    [Authorize]
    public class InfoController : ApiController
    {
        [HttpGet]
        [Route("geterror")]
        public IHttpActionResult getone()
        {
            ErrorLogs response = new ErrorLogs();

            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                response = db.ErrorLogs.Take(1).OrderByDescending(x => x.FechaDelError).FirstOrDefault();
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("geterrors")]
        public IHttpActionResult getall()
        {
            List<ErrorLogs> response = new List<ErrorLogs>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                response = db.ErrorLogs.ToList();
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("getallpending")]
        public IHttpActionResult getallga()
        {
            List<General_Audit> response = new List<General_Audit>();

            using (DevelopmentDbContext db = new DevelopmentDbContext())
            {
                response = db.General_Audit.ToList();
            }

            return Ok(response);
        }
    }
}

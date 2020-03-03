using IntegrationWS.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IntegrationWS.WSMovilDGII;
using System.ServiceModel;
using IntegrationWS.Utils;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Models;
using IntegrationWS.Data;
using System.Web.Script.Serialization;
using IntegrationWS.ModelsNotMapped;

namespace IntegrationWS.Controllers
{
    [RoutePrefix("api/comisionmedica")]
    public class ComisionMedicaController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult get([FromBody] List<ComisionMedicaDTO> comisionMedicaDTO)
        {
            try
            {
                if (comisionMedicaDTO == null)
                {
                    ModelState.AddModelError("Message", "El body no debe ser nulo.");
                    return BadRequest(ModelState);
                }

                Pedido pedido = new Pedido();
                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(comisionMedicaDTO);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                List<ComisionMedicaDTO> result = new List<ComisionMedicaDTO>();
                using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
                {
                    foreach (ComisionMedicaDTO comision in comisionMedicaDTO)
                    {
                        db_dev.Database.ExecuteSqlCommand($"ADD_Previsiones_Medica " +
                            $"'{comision.SalesPersonId}', " +
                            $"'{comision.IsVaccines}', " +
                            $"'{comision.Year}', " +
                            $"'{comision.Month}', " +
                            $"'{comision.Quote}', " +
                            $"'{comision.BaseAward}'");
                    }

                    string mes = comisionMedicaDTO.Select(x => x.Month).FirstOrDefault();
                    string year = comisionMedicaDTO.Select(x => x.Year).FirstOrDefault();
                    result = db_dev.Database.SqlQuery<ComisionMedicaDTO>($"EXEC ComisionesMedica '{mes}', '{year}'").ToList();
                }

                return Content(HttpStatusCode.Created, result);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Message", e.Message.ToString());
                return BadRequest(ModelState);
            }
            
        }
    }
}

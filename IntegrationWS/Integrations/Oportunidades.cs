using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class Oportunidades : IOportunidades
    {

        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;        
        private readonly ISobjectCRUD<OpportunitySf> _sobjectCRUD;
        private readonly ISobjectCRUD<OpportunityToUpdate> _sobjectCRUD2;
        private readonly ISobjectCRUD<OpportunityTeamMember> _sobjectCRUD3;
        private readonly ISobjectCRUD<OpportunitySplit> _sobjectCRUD4;
        private readonly string sobject;

        public Oportunidades(IAuthToSalesforce authToSalesforce, 
                             ISobjectCRUD<OpportunitySf> sobjectCRUD, 
                             ISobjectCRUD<OpportunityToUpdate> sobjectCRUD2, 
                             ISobjectCRUD<OpportunityTeamMember> sobjectCRUD3,
                             ISobjectCRUD<OpportunitySplit> sobjectCRUD4,
                             IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            _sobjectCRUD2 = sobjectCRUD2;
            _sobjectCRUD3 = sobjectCRUD3;
            _sobjectCRUD4 = sobjectCRUD4;
            sobject = "Opportunity";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            OpportunitySf opportunity = await getOne(Id, loginResult);

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var ListaDePrecio = await _sobjectCRUD.rawQuery5(loginResult, opportunity, opportunity.AccountId, sobject);

                if (ListaDePrecio.Trim() != "")
                {
                    opportunity.Pricebook2Id = db.Lista_De_Precios.Where(x => x.DynamicsId == ListaDePrecio).Select(x => x.SalesforceId).FirstOrDefault();
                }
                else
                {
                    if (opportunity.CurrencyIsoCode == "DOP")
                    {
                        opportunity.Pricebook2Id = db.Lista_De_Precios.Where(x => x.DynamicsId == "LISTA_DOP").Select(x => x.SalesforceId).FirstOrDefault();
                    }
                    else if (opportunity.CurrencyIsoCode == "USD")
                    {
                        opportunity.Pricebook2Id = db.Lista_De_Precios.Where(x => x.DynamicsId == "LISTA_USD").Select(x => x.SalesforceId).FirstOrDefault();
                    }
                }
            }


            var result = await _sobjectCRUD.addSobjectAsync(loginResult, opportunity, sobject);

            //var logoutResult = await _authToSalesforce.Logout(authToken, serviceURL);
            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery2(loginResult, opportunity, Id, sobject);

                Oportunidad oportunidad = new Oportunidad();
                oportunidad.DynamicsId = Id;
                oportunidad.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Oportunidad.Add(oportunidad);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL);

                if (result2 != "Ok")
                {
                    return result2;
                }

                return "actualizado";
            }

            if (result.Contains("errorCode"))
            {
                return result;
            }

            //BEGIN Esta parte es la que se encarga se actualizar los equipos de Oportunidades y división de oportunidades
            List<OpportunityTeamMember> opportunityTeamMemberList = new List<OpportunityTeamMember>();
            //List<OpportunitySplit> opportunitySplitList = new List<OpportunitySplit>();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                opportunityTeamMemberList = db_dev.Database.SqlQuery<OpportunityTeamMember>($"EXEC DEVELOPMENT.[dbo].[SP_GPSalesforce_OpportunityTeamMember_2] '{Id}'").ToList();
               // opportunitySplitList = db_dev.Database.SqlQuery<OpportunitySplit>($"EXEC DEVELOPMENT.[dbo].[SP_GPSalesforce_OpportunitySplit_2] '{Id}'").ToList();
            }

            foreach (var oppTeamMmbr in opportunityTeamMemberList)
            {
                var result2 = _sobjectCRUD3.addSobjectAsync(loginResult, oppTeamMmbr, "OpportunityTeamMember");
            }

           // foreach (var oppDiv in opportunitySplitList)
         //   {
           //     var result2 = _sobjectCRUD4.addSobjectAsync(loginResult, oppDiv, "OpportunitySplit");
          //  }
            //END

            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL)
        {
            OpportunitySf opportunity = await getOne(Id, loginResult);
            string salesforceID = string.Empty;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Oportunidad.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();

                var ListaDePrecio = await _sobjectCRUD.rawQuery5(loginResult, opportunity, opportunity.AccountId, sobject);

                if (ListaDePrecio.Trim() != "")
                {
                    opportunity.Pricebook2Id = db.Lista_De_Precios.Where(x => x.DynamicsId == ListaDePrecio).Select(x => x.SalesforceId).FirstOrDefault();
                }
                else
                {
                    if(opportunity.CurrencyIsoCode == "DOP")
                    {
                        opportunity.Pricebook2Id = db.Lista_De_Precios.Where(x => x.DynamicsId == "LISTA_DOP").Select(x => x.SalesforceId).FirstOrDefault();
                    }
                    else if(opportunity.CurrencyIsoCode == "USD")
                    {
                        opportunity.Pricebook2Id = db.Lista_De_Precios.Where(x => x.DynamicsId == "LISTA_USD").Select(x => x.SalesforceId).FirstOrDefault();
                    }                    
                }
            }

            
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, opportunity, salesforceID, sobject);

            if(result != "Ok")
            {
                JArray jsonArray = JArray.Parse(result);
                result = jsonArray[0].ToString();
                JObject obj3 = JObject.Parse(result);
                result = (string)obj3["errorCode"];

                if (result == "FIELD_INTEGRITY_EXCEPTION")
                {
                    OpportunityToUpdate oppToUpdate = new OpportunityToUpdate();
                    oppToUpdate.Description = opportunity.Description;
                    oppToUpdate.StageName = opportunity.StageName;
                    oppToUpdate.AccountId = opportunity.AccountId;
                    oppToUpdate.Name = opportunity.Name;
                    oppToUpdate.NextStep = opportunity.NextStep;
                    oppToUpdate.Probability = opportunity.Probability;
                    oppToUpdate.OwnerId = opportunity.OwnerId;
                    oppToUpdate.Type = opportunity.Type;
                    oppToUpdate.Oportunidad_Vendedor__c = opportunity.Oportunidad_Vendedor__c;
                    oppToUpdate.CloseDate = opportunity.CloseDate;
                    oppToUpdate.Referencia__c = opportunity.Referencia__c;
                    oppToUpdate.Factura_de_origen__c = opportunity.Factura_de_origen__c;
                    oppToUpdate.URL_Factura_de_origen__c = opportunity.URL_Factura_de_origen__c;
                    oppToUpdate.PaymentTerm__c = opportunity.PaymentTerm__c;

                    result = await _sobjectCRUD2.updateSobjectByIdAsync(loginResult, oppToUpdate, salesforceID, sobject);
                }

                if (result != "Ok")
                {
                    return result;
                }
            }

            //BEGIN Esta parte es la que se encarga se actualizar los equipos de oportunidades
            List<OpportunityTeamMember> opportunityTeamMemberList = new List<OpportunityTeamMember>();
            List<EquiposDeOportunidad> opportunityTeamMemberListLocal = new List<EquiposDeOportunidad>();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                opportunityTeamMemberList = db_dev.Database.SqlQuery<OpportunityTeamMember>($"EXEC DEVELOPMENT.[dbo].[SP_GPSalesforce_OpportunityTeamMember_2] '{Id.Trim()}'").ToList();
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                opportunityTeamMemberListLocal = db.EquiposDeOportunidad.Where(x => x.OpportunityId == salesforceID).ToList();
            }


            List<string> Ids = new List<string>();
            foreach (var acc in opportunityTeamMemberList)
            {
                Ids.Add(acc.UserId);
            }

            //Verfificar si existen nuevos
            foreach (var oppTeamFromDb in opportunityTeamMemberList)
            {
                foreach (var oppTeamInSf in opportunityTeamMemberListLocal)
                {
                    if (oppTeamFromDb.UserId == oppTeamInSf.UserId)
                    {
                        var found = Ids.Find(x => x == oppTeamFromDb.UserId);
                        Ids.Remove(found);
                    }
                }
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (var Id_ in Ids)
                {
                    EquiposDeOportunidad equiposDeOportunidad = new EquiposDeOportunidad();
                    var oppTeam = opportunityTeamMemberList.Find(x => x.UserId == Id_);
                    var result2 = _sobjectCRUD3.addSobjectAsync(loginResult, oppTeam, "OpportunityTeamMember");
                    equiposDeOportunidad.SalesforceId = await _sobjectCRUD3.TeamMemberId(loginResult, oppTeam.OpportunityId, oppTeam.UserId, "OpportunityTeamMember", "opp");
                    equiposDeOportunidad.TeamMemberRole = oppTeam.TeamMemberRole;
                    equiposDeOportunidad.UserId = oppTeam.UserId;
                    equiposDeOportunidad.OpportunityId = oppTeam.OpportunityId;
                    db.EquiposDeOportunidad.Add(equiposDeOportunidad);
                }
                db.SaveChanges();

                opportunityTeamMemberListLocal = db.EquiposDeOportunidad.Where(x => x.OpportunityId == salesforceID).ToList();
            }


            List<string> Ids2 = new List<string>();
            foreach (var acc in opportunityTeamMemberListLocal)
            {
                Ids2.Add(acc.UserId);
            }

            //Eliminar los que ya no esten asociados al cliente
            foreach (var oppTeamInSf in opportunityTeamMemberListLocal)
            {
                foreach (var oppTeamFromDb in opportunityTeamMemberList)
                {
                    if (oppTeamFromDb.UserId == oppTeamInSf.UserId)
                    {
                        var found = Ids2.Find(x => x == oppTeamFromDb.UserId);
                        Ids2.Remove(found);
                    }
                }
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (var Id2_ in Ids2)
                {
                    EquiposDeOportunidad equiposDeOportunidad = new EquiposDeOportunidad();
                    var oppTeam = opportunityTeamMemberListLocal.Find(x => x.UserId == Id2_);
                    var result2 = await _sobjectCRUD.deleteSobjectByIdAsync(loginResult, oppTeam.SalesforceId, "OpportunityTeamMember");

                    equiposDeOportunidad = db.EquiposDeOportunidad.Where(x => x.UserId == oppTeam.UserId).FirstOrDefault();
                    db.EquiposDeOportunidad.Remove(equiposDeOportunidad);
                }

                db.SaveChanges();
            }
            //END equipos de cuentas

            ////BEGIN division de oportunidades
            //List<OpportunitySplit> opportunitySplitList = new List<OpportunitySplit>();

            //using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            //{
            //    opportunitySplitList = db_dev.Database.SqlQuery<OpportunitySplit>($"EXEC DEVELOPMENT.[dbo].[SP_GPSalesforce_OpportunitySplit_2] '{Id}'").ToList();
            //}

            //foreach (var oppDiv in opportunitySplitList)
            //{
            //    var result2 = _sobjectCRUD4.addSobjectAsync(loginResult, oppDiv, "OpportunitySplit");
            //}
            ////END division de oportunidades

            return "Ok";
        }

        public async Task<string> delete(string loginResult, string Id)
        {

            var result = await _sobjectCRUD.deleteSobjectByIdAsync(loginResult, Id, sobject);
            
            if (result == "Ok")
            {
                return "Ok";
            }
            
            return result;
        }


        public async Task<OpportunitySf> getOne(string Id, string loginResult)
        {
            OpportunitySf oportunidad = new OpportunitySf();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                oportunidad = db_dev.Database.SqlQuery<OpportunitySf>($"SP_GPSalesforce_Opportunity_BySopNumbe '{Id}'").FirstOrDefault();
            }

            if(oportunidad.Factura_de_origen__c != null)
            {
                using(ApplicationDbContext db = new ApplicationDbContext())
                {
                    string IdOrigen = db.Oportunidad.Where(x => x.DynamicsId == oportunidad.Factura_de_origen__c).Select(x => x.SalesforceId).FirstOrDefault();

                    if(IdOrigen == null)
                    {
                        IdOrigen = await _sobjectCRUD.rawQuery2(loginResult, oportunidad, oportunidad.Factura_de_origen__c.Trim(), "Opportunity");
                    }

                    oportunidad.URL_Factura_de_origen__c = $"https://bionuclear.lightning.force.com/lightning/r/Opportunity/{IdOrigen}/view";
                }
            }

            //Esto será removido cuando se usen dolares
            oportunidad.CurrencyIsoCode = "DOP";

            return oportunidad;
        }
    }
}
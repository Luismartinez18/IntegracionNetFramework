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
    public class ProductoDeOportunidad : IProductoDeOportunidad
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<OpportunityLineItem> _sobjectCRUD;
        private readonly string sobject;

        public ProductoDeOportunidad(IAuthToSalesforce authToSalesforce, ISobjectCRUD<OpportunityLineItem> sobjectCRUD, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "OpportunityLineItem";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            List<OpportunityLineItem> opportunityLineItemList = getOne(Id);
            
            var result = string.Empty;

            foreach (OpportunityLineItem opportunityLineItem in opportunityLineItemList)
            {     
                result = await _sobjectCRUD.addSobjectAsync(loginResult, opportunityLineItem, sobject);

                if(result.Contains("versions 3.0 and higher must specify pricebook entry id"))
                {
                    string listaDePrecios;
                    string producto;
                    string oportunidad;

                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        oportunidad = db.Oportunidad.Where(x => x.SalesforceId == opportunityLineItem.OpportunityId).Select(x => x.DynamicsId).FirstOrDefault();                        

                        listaDePrecios = "LISTA_DOP";

                        producto = db.Productos.Where(x => x.SalesforceId == opportunityLineItem.Product2Id).Select(x => x.DynamicsId).FirstOrDefault();
                    }

                    using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
                    {
                        General_Audit newForSOP30300 = new General_Audit();
                        newForSOP30300.Activity = "INSERT";
                        newForSOP30300.DateOfChanged = DateTime.Now;
                        newForSOP30300.DoneBy = "integrationgp";
                        newForSOP30300.DynamicsId = $"{listaDePrecios} | {producto}";
                        newForSOP30300.HasChanged = 1;
                        newForSOP30300.TableName = "IV10402";
                        db_dev.General_Audit.Add(newForSOP30300);
                        db_dev.SaveChanges();
                    }

                    result = "errorCode";
                    return result;
                }                

                if (result.Contains("DUPLICATE"))
                {
                    result = "Duplicado";
                    continue;
                }

                if (result.Contains("errorCode"))
                {
                    return result;
                }
            }
            
            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL)
        {
            List<OpportunityLineItem> opportunityLineItemList = getOne(Id);
            string salesforceID = string.Empty;
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Producto_de_oportunidad.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }

            var result = string.Empty;
            foreach (OpportunityLineItem opportunityLineItem in opportunityLineItemList)
            {
                result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, opportunityLineItem, salesforceID, sobject);
            }

            if (result != "Ok")
            {
                return result;
            }

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


        public List<OpportunityLineItem> getOne(string Id)
        {
            OpportunitySf oportunidad = new OpportunitySf();
            List<OpportunityLineItem> opportunityLineItem = new List<OpportunityLineItem>();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                oportunidad = db_dev.Database.SqlQuery<OpportunitySf>($"SP_GPSalesforce_Opportunity_BySopNumbe '{Id.Trim()}'").FirstOrDefault();

                if (oportunidad != null) 
                {
                    if (!oportunidad.Name.Contains("*"))
                    {
                        opportunityLineItem = db_dev.Database.SqlQuery<OpportunityLineItem>($"SP_GPSalesforce_OpportunityLineItem_V2 '{Id}'").ToList();
                    }
                }      
            }

            return opportunityLineItem;
        }
    }
}
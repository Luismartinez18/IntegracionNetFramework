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
    public class ProductoDeContrato : IProductoDeContrato
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<ContratoLineItem> _sobjectCRUD;
        private readonly string sobject;

        public ProductoDeContrato(IAuthToSalesforce authToSalesforce, ISobjectCRUD<ContratoLineItem> sobjectCRUD, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Contrato_de_servicio_Detalle__c";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            List<ContratoLineItem> contratoLineItemList = getOne(Id);
            
            var result = string.Empty;

            foreach (ContratoLineItem contratoLineItem in contratoLineItemList)
            {     
                result = await _sobjectCRUD.addSobjectAsync(loginResult, contratoLineItem, sobject);

                if(result.Contains("versions 3.0 and higher must specify pricebook entry id"))
                {
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
            List<ContratoLineItem> contratoLineItemList = getOne(Id);
            string salesforceID = string.Empty;
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Producto_de_contrato.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }

            var result = string.Empty;
            foreach (ContratoLineItem contratoLineItem in contratoLineItemList)
            {
                result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, contratoLineItem, salesforceID, sobject);
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

        public List<ContratoLineItem> getOne(string Id)
        {
            ContractSf contrato = new ContractSf();
            List<ContratoLineItem> contratoLineItem = new List<ContratoLineItem>();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                contrato = db_dev.Database.SqlQuery<ContractSf>($"SP_GPSalesforce_Contract_ByContnbr '{Id.Trim()}'").FirstOrDefault();

                if (contrato != null) 
                {
                    contratoLineItem = db_dev.Database.SqlQuery<ContratoLineItem>($"SP_GPSalesforce_ContratoLineItem '{Id}'").ToList();
                }      
            }

            return contratoLineItem;
        }
    }
}
using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
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
    public class ProductoConLoteUtils : IProductoConLoteUtils
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<ProductoConLote> _sobjectCRUD;
        private readonly ISobjectCRUD<ProductoConLoteUpdate> _sobjectCRUD2;
        private readonly string sobject;

        public ProductoConLoteUtils(IAuthToSalesforce authToSalesforce, 
                                    ISobjectCRUD<ProductoConLote> sobjectCRUD,
                                    ISobjectCRUD<ProductoConLoteUpdate> sobjectCRUD2,
                                    IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            _sobjectCRUD2 = sobjectCRUD2;
            sobject = "Producto_con_lote__c";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            List<ProductoConLote> ProductoConLoteList = getOne(Id);
            
            var result = string.Empty;

            foreach (ProductoConLote productoConLote in ProductoConLoteList)
            {
                result = await _sobjectCRUD.addSobjectAsync(loginResult, productoConLote, sobject);                

                if (result.Contains("DUPLICATE"))
                {
                    result = "Ok";
                    continue;
                    /*ProductoConLoteUpdate productoConLoteUpdate = new ProductoConLoteUpdate()
                    {
                        Cantidad__c = productoConLote.Cantidad__c,
                        Producto__c = productoConLote.Producto__c,
                        Precio_Unitario__c = productoConLote.Precio_Unitario__c,
                        Lote_o_Serie__c = productoConLote.Lote_o_Serie__c,
                        Id_External__c = productoConLote.Id_External__c
                    };

                    JArray jsonArray = JArray.Parse(result);
                    result = jsonArray[0].ToString();
                    JObject obj3 = JObject.Parse(result);
                    result = (string)obj3["message"];

                    var salesforceId = result.Substring(result.IndexOf("Id.:") + 4, 16).Trim();
                    result = await _sobjectCRUD2.updateSobjectByIdAsync(loginResult, productoConLoteUpdate, salesforceId, sobject);*/
                }

                if (result.Contains("errorCode"))
                {
                    return result;
                }
            }

            return result;
        }

        public List<ProductoConLote> getOne(string Id)
        {
            OpportunitySf oportunidad = new OpportunitySf();
            List<ProductoConLote> ProductoConLoteList = new List<ProductoConLote>();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                oportunidad = db_dev.Database.SqlQuery<OpportunitySf>($"SP_GPSalesforce_Opportunity_BySopNumbe '{Id.Trim()}'").FirstOrDefault();

                if (oportunidad == null)
                {
                    return ProductoConLoteList;
                }

                if (!oportunidad.Name.Contains("*"))
                {
                    ProductoConLoteList = db_dev.Database.SqlQuery<ProductoConLote>($"SP_GPSalesforce_OpportunityLineItemWithLot '{Id}'").ToList();
                }
            }

            return ProductoConLoteList;
        }
    }
}
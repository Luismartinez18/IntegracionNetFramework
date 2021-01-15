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
    public class PedidoServices : IPedidos
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;        
        private readonly ISobjectCRUD<PedidoSf> _sobjectCRUD;
        private readonly string sobject;

        public PedidoServices(IAuthToSalesforce authToSalesforce, 
                              ISobjectCRUD<PedidoSf> sobjectCRUD, 
                              IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Pedidos__c";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            PedidoSf PedidoSf = await getOne(Id, loginResult);

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, PedidoSf, sobject);
            var SalesforceId = string.Empty;
;
            if (result.Contains("DUPLICATE"))
            {
                SalesforceId = await _sobjectCRUD.rawQuery6(loginResult, PedidoSf, Id, sobject);

                Pedidos Pedidos = new Pedidos();
                Pedidos.DynamicsId = Id;
                Pedidos.SalesforceId = SalesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedido.Add(Pedidos);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL, SalesforceId);

                if (result2 != "Ok")
                {
                    return result2;
                }

                return "actualizado";
            }
            else if (!result.Contains("errorCode"))
            {
                JObject obj2 = JObject.Parse(result);
                SalesforceId = (string)obj2["id"];

                Pedidos pedido = new Pedidos();
                pedido.DynamicsId = Id;
                pedido.SalesforceId = SalesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedido.Add(pedido);
                    db.SaveChanges();
                }
            }

            if (result.Contains("errorCode"))
            {
                return result;
            }

            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL, string SalesforceId)
        {
            PedidoSf pedido = await getOne(Id, loginResult);

            //pedido.Pricebook2Id = null;
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, pedido, SalesforceId, sobject);

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

        public async Task<PedidoSf> getOne(string Id, string loginResult)
        {
            PedidoSf pedido = new PedidoSf();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                pedido = db_dev.Database.SqlQuery<PedidoSf>($"SP_GPSalesforce_Pedido_BySopNumbe '{Id}'").FirstOrDefault();
            }

            //if(pedido.Factura_de_origen__c != null)
            //{
            //    using(ApplicationDbContext db = new ApplicationDbContext())
            //    {
            //        string IdOrigen = db.Pedido.Where(x => x.DynamicsId == pedido.Factura_de_origen__c).Select(x => x.SalesforceId).FirstOrDefault();

            //        if(IdOrigen == null)
            //        {
            //            IdOrigen = await _sobjectCRUD.rawQuery2(loginResult, pedido, pedido.Factura_de_origen__c.Trim(), "Order");
            //        }

            //        pedido.URL_Factura_de_origen__c = $"https://bionuclear.lightning.force.com/lightning/r/Order/{IdOrigen}/view";
            //    }
            //}

            return pedido;
        }
    }
}
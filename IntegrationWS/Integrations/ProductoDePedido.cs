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
    public class ProductoDePedido : IProductoDePedido
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<PedidoLineItem> _sobjectCRUD;
        private readonly string sobject;

        public ProductoDePedido(IAuthToSalesforce authToSalesforce, ISobjectCRUD<PedidoLineItem> sobjectCRUD, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "PedidoItem__c";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            List<PedidoLineItem> pedidoLineItemList = getOne(Id);
            
            var result = string.Empty;

            foreach (PedidoLineItem pedidoLineItem in pedidoLineItemList)
            {     
                result = await _sobjectCRUD.addSobjectAsync(loginResult, pedidoLineItem, sobject);

                if(result.Contains("versions 3.0 and higher must specify pricebook entry id"))
                {
                    string producto;
                    string Order;

                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        Order = db.Pedido.Where(x => x.SalesforceId == pedidoLineItem.Pedidos__c).Select(x => x.DynamicsId).FirstOrDefault();                        

                        producto = db.Productos.Where(x => x.SalesforceId == pedidoLineItem.Producto__c).Select(x => x.DynamicsId).FirstOrDefault();
                    }

                    //using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
                    //{
                    //    General_Audit newForSOP30300 = new General_Audit();
                    //    newForSOP30300.Activity = "INSERT";
                    //    newForSOP30300.DateOfChanged = DateTime.Now;
                    //    newForSOP30300.DoneBy = "integrationgp";
                    //    newForSOP30300.DynamicsId = $"{listaDePrecios} | {producto}";
                    //    newForSOP30300.HasChanged = 1;
                    //    newForSOP30300.TableName = "IV10402";
                    //    db_dev.General_Audit.Add(newForSOP30300);
                    //    db_dev.SaveChanges();
                    //}

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
            List<PedidoLineItem> pedidoLineItemList = getOne(Id);
            string salesforceID = string.Empty;
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Producto_de_pedido.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }

            var result = string.Empty;
            foreach (PedidoLineItem pedidoLineItem in pedidoLineItemList)
            {
                result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, pedidoLineItem, salesforceID, sobject);
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

        public List<PedidoLineItem> getOne(string Id)
        {
            PedidoSf pedido = new PedidoSf();
            List<PedidoLineItem> pedidoLineItem = new List<PedidoLineItem>();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                pedido = db_dev.Database.SqlQuery<PedidoSf>($"SP_GPSalesforce_Pedido_BySopNumbe '{Id.Trim()}'").FirstOrDefault();

                if (pedido != null) 
                {
                    pedidoLineItem = db_dev.Database.SqlQuery<PedidoLineItem>($"SP_GPSalesforce_PedidoLineItem_V2 '{Id}'").ToList();
                }      
            }

            return pedidoLineItem;
        }
    }
}
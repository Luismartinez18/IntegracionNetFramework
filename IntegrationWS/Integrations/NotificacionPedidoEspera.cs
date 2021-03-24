using IntegrationWS.Data;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class NotificacionPedidoEspera : INotificacionPedidoEspera
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<NotificacionPedidosEnEsperaSf> _sobjectCRUD;
        private readonly string sobject;

        public NotificacionPedidoEspera(IAuthToSalesforce authToSalesforce, 
                                         ISobjectCRUD<NotificacionPedidosEnEsperaSf> sobjectCRUD, 
                                         IResponseAfterAuth responseAfterAuth
                                         )
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Notificacion_de_pedidos_en_espera__c";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            NotificacionPedidosEnEsperaSf product = getOne(Id);
            
            var result = await _sobjectCRUD.addSobjectAsync(loginResult, product, sobject);
            
            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery(loginResult, product, Id, sobject);

                Notificacion_de_pedido_espera notificacion = new Notificacion_de_pedido_espera();
                notificacion.DynamicsId = Id;
                notificacion.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Notificacion_de_pedido_espera.Add(notificacion);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL);

                if(result2 != "Ok")
                {
                    return result2;
                }
                
                return "actualizado";
            }
            else if (result.Contains("errorCode"))
            {
                return result;
            }

            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL)
        {
            NotificacionPedidosEnEsperaSf product = getOne(Id);
            string salesforceID = string.Empty;

            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Notificacion_de_pedido_espera.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, product, salesforceID, sobject);

            if (result != "Ok")
            {                
                return result;
            }
            
            return "Ok";
        }

        public async Task<string> delete(string loginResult, string Id, string DynamicsId)
        {

            var result = await _sobjectCRUD.deleteSobjectByIdAsync(loginResult, Id, sobject);

            if (result == "Ok")
            {
                return "Ok";
            }

            return result;
        }

        public NotificacionPedidosEnEsperaSf getOne(string Id)
        {
            NotificacionPedidosEnEsperaSf notificacion = new NotificacionPedidosEnEsperaSf();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                notificacion = db_dev.Database.SqlQuery<NotificacionPedidosEnEsperaSf>($"SP_GPSalesforce_NotificacionPedidoEspera_ById '{Id}'").FirstOrDefault();
            }

            return notificacion;
        }
    }
}
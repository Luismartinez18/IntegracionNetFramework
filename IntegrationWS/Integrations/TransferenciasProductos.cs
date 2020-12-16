using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class TransferenciasProductos : ITransferenciasProductos
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<ProductTransfer> _sobjectCRUD;
        private readonly string sobject;

        public TransferenciasProductos(IAuthToSalesforce authToSalesforce, ISobjectCRUD<ProductTransfer> sobjectCRUD, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "ProductTransfer";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            ProductTransfer productTransfer = getOne(Id);
            
            var result = await _sobjectCRUD.addSobjectAsync(loginResult, productTransfer, sobject);
            
            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery2(loginResult, productTransfer, Id, sobject);

                TransferenciaProducto transferencia = new TransferenciaProducto();
                transferencia.DynamicsId = Id;
                transferencia.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.TransferenciaProducto.Add(transferencia);
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
            
            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL)
        {
            ProductTransfer productTransfer = getOne(Id);
            string salesforceID = string.Empty;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.TransferenciaProducto.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, productTransfer, salesforceID, sobject);
            
            if (result != "Ok")
            {
                return result;
            }
            
            return "Ok";
        }

        public ProductTransfer getOne(string Id)
        {
            var transferId = Id.Substring(0, Id.IndexOf('|')).Trim();
            var posicion = Id.IndexOf('|');
            var posicion2 = Id.IndexOf('|') + 1;
            var itemId = Id.Substring(posicion2).Trim();            

            ProductTransfer productTransfer = new ProductTransfer();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                productTransfer = db_dev.Database.SqlQuery<ProductTransfer>($"SP_GPSalesforce_ProductTransfer_ByITEMNMBR '{transferId}', '{itemId}'").FirstOrDefault();
                if (productTransfer != null)
                    productTransfer.Id_External__c = Id;
            }

            return productTransfer;
        }
    }
}
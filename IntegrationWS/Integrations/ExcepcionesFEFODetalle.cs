using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class ExcepcionesFEFODetalle : IExcepcionesFEFODetalle
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<Detalle_de_Excepci_n_FEFO__c> _sobjectCRUD;
        private readonly string sobject;

        public ExcepcionesFEFODetalle(IAuthToSalesforce authToSalesforce,
                         ISobjectCRUD<Detalle_de_Excepci_n_FEFO__c> sobjectCRUD,
                         IResponseAfterAuth responseAfterAuth
                         )
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Detalle_de_Excepci_n_FEFO__c";
        }

        public async Task<string> create(string Id, 
                                        string loginResult, 
                                        string authToken, 
                                        string serviceURL, 
                                        string SfId)
        {
            List<Detalle_de_Excepci_n_FEFO__c> excepcionesDetalles = getAll(Id);
            string result = string.Empty;

            foreach(var detalleExcepcion in excepcionesDetalles)
            {
                detalleExcepcion.Excepci_n_FEFO__c = SfId;

                result = await _sobjectCRUD.addSobjectAsync(loginResult, detalleExcepcion, sobject);

                if (result.Contains("DUPLICATE"))
                {
                    var salesforceId = await _sobjectCRUD.rawQuery(loginResult, detalleExcepcion, detalleExcepcion.Id_External__c, sobject);

                    var result2 =  await _sobjectCRUD.updateSobjectByIdAsync(loginResult, detalleExcepcion, salesforceId, sobject);

                    if (result2 != "Ok")
                    {
                        return result2;
                    }

                    return "actualizado";
                }
                else if (result.Contains("errorCode"))
                {
                    return result;
                }
            }

            return result;
        }

        public async Task<string> delete(string loginResult, string DynamicsId)
        {
            var salesforceId = await _sobjectCRUD.rawQuery(loginResult, null, DynamicsId, sobject);
            var result = await _sobjectCRUD.deleteSobjectByIdAsync(loginResult, salesforceId, sobject);

            if (result == "Ok")
            {
                return "Ok";
            }

            return result;
        }

        public List<Detalle_de_Excepci_n_FEFO__c> getAll(string Id)
        {
            List<Detalle_de_Excepci_n_FEFO__c> excepcionFefoDetalleList = new List<Detalle_de_Excepci_n_FEFO__c>();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                excepcionFefoDetalleList = db_dev.Database.SqlQuery<Detalle_de_Excepci_n_FEFO__c>($"SP_GPSalesforce_ExcepcionesFEFO_Detalle '{Id.Trim()}'").ToList();
            }

            return excepcionFefoDetalleList;
        }
    }
}
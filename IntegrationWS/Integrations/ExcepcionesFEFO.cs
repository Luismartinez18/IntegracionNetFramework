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
    public class ExcepcionesFEFO : IExcepcionesFEFO
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<Excepci_n_FEFO__c> _sobjectCRUD;
        private readonly string sobject;

        public ExcepcionesFEFO(IAuthToSalesforce authToSalesforce,
                         ISobjectCRUD<Excepci_n_FEFO__c> sobjectCRUD,
                         IResponseAfterAuth responseAfterAuth
                         )
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Excepci_n_FEFO__c";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            Excepci_n_FEFO__c excepcion = getOne(Id);

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, excepcion, sobject);

            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery(loginResult, excepcion, Id, sobject);

                ViolacionesFEFO excep = new ViolacionesFEFO();
                excep.DynamicsId = Id;
                excep.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.ViolacionesFEFO.Add(excep);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL);

                if (result2 != "Ok")
                {
                    return result2;
                }

                return $"actualizado {salesforceId}";
            }
            else if (result.Contains("errorCode"))
            {
                return result;
            }

            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL)
        {
            Excepci_n_FEFO__c excepcion = getOne(Id);

            string salesforceID = string.Empty;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.ViolacionesFEFO.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, excepcion, salesforceID, sobject);

            if (result != "Ok")
            {
                return result;
            }

            return $"Ok {salesforceID}";
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

        public Excepci_n_FEFO__c getOne(string Id)
        {
            Excepci_n_FEFO__c excepcionFefo = new Excepci_n_FEFO__c();
            
            using(DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                excepcionFefo = db_dev.Database.SqlQuery<Excepci_n_FEFO__c>($"EXEC SP_GPSalesforce_ExcepcionesFEFO '{Id.Trim()}'").FirstOrDefault();
            }

            return excepcionFefo;
        }

    }
}
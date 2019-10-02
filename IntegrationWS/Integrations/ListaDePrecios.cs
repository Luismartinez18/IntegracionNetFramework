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
    public class ListaDePrecios : IListaDePrecios
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<Pricebook2> _sobjectCRUD;
        private readonly string sobject;

        public ListaDePrecios(IAuthToSalesforce authToSalesforce, ISobjectCRUD<Pricebook2> sobjectCRUD, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Pricebook2";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            Pricebook2 pricebook2 = getOne(Id);

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, pricebook2, sobject);
            
            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery3(loginResult, pricebook2, Id, sobject);

                Lista_De_Precios listaDePrecios = new Lista_De_Precios();
                listaDePrecios.DynamicsId = Id;
                listaDePrecios.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Lista_De_Precios.Add(listaDePrecios);
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
            Pricebook2 pricebook2 = getOne(Id);
            string salesforceID = string.Empty;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Lista_De_Precios.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, pricebook2, salesforceID, sobject);
            

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


        public Pricebook2 getOne(string Id)
        {
            Pricebook2 pricebook2 = new Pricebook2();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                pricebook2 = db_dev.Database.SqlQuery<Pricebook2>($"SP_GPSalesforce_Integracion_Pricebook2_V2 '{Id}'").FirstOrDefault();

                if (pricebook2.IsActive == "0")
                {
                    pricebook2.IsActive = "false";
                }
                else
                {
                    pricebook2.IsActive = "true";
                }
            }

            return pricebook2;
        }
    }
}
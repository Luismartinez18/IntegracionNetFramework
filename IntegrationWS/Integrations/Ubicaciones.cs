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
    public class Ubicaciones : IUbicaciones
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<LocationSf> _sobjectCRUD;
        private readonly string sobject;

        public Ubicaciones(IAuthToSalesforce authToSalesforce, ISobjectCRUD<LocationSf> sobjectCRUD, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Location";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            LocationSf location = getOne(Id);
            
            var result = await _sobjectCRUD.addSobjectAsync(loginResult, location, sobject);
            
            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery(loginResult, location, Id, sobject);

                Ubicacion ubicacion = new Ubicacion();
                ubicacion.DynamicsId = Id;
                ubicacion.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Ubicaciones.Add(ubicacion);
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
            LocationSf location = getOne(Id);
            string salesforceID = string.Empty;
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Ubicaciones.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, location, salesforceID, sobject);
            
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


        public LocationSf getOne(string Id)
        {
            CompanyKey companyKey;
            Context context;

            //Declarando los objetos necesarios para el get de las transferencias
            Warehouse warehouse;
            WarehouseKey warehouseKey;

            //Inicializando el objeto que voy a enviar a salesforce
            LocationSf location = new LocationSf();

            // Create an instance of the service 
            DynamicsGPClient wsDynamicsGP = new DynamicsGPClient();

            // Create a context with which to call the web service 
            context = new Context();

            // Specify which company to use (lesson company) 
            companyKey = new CompanyKey();
            companyKey.Id = (1);

            // Set up the context 
            context.OrganizationKey = (OrganizationKey)companyKey;

            // Create a Inventory key 
            warehouseKey = new WarehouseKey();
            warehouseKey.Id = Id;

            warehouse = wsDynamicsGP.GetWarehouseByKey(warehouseKey, context);           

            // Close the service 
            if (wsDynamicsGP.State != CommunicationState.Faulted)
            {
                wsDynamicsGP.Close();
            }
            
            //Completando el objeto
            location.Id_External__c = Id;
            location.Name = warehouse.Description;
            location.IsInventoryLocation = true;
            location.IsMobile = true;
            location.Description = warehouse.Description;

            return location;
        }
    }
}
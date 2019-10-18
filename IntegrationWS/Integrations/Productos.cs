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
    public class Productos : IProductos
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<Product2> _sobjectCRUD;
        private readonly ISobjectCRUD<Product2ToDelete> _sobjectCRUD2;
        private readonly string sobject;

        public Productos(IAuthToSalesforce authToSalesforce, 
                         ISobjectCRUD<Product2> sobjectCRUD, 
                         IResponseAfterAuth responseAfterAuth,
                         ISobjectCRUD<Product2ToDelete> sobjectCRUD2
                         )
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            _sobjectCRUD2 = sobjectCRUD2;
            sobject = "Product2";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            Product2 product = getOne(Id);
            
            var result = await _sobjectCRUD.addSobjectAsync(loginResult, product, sobject);
            
            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery(loginResult, product, Id, sobject);

                Producto producto = new Producto();
                producto.DynamicsId = Id;
                producto.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Productos.Add(producto);
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
            Product2 product = getOne(Id);
            string salesforceID = string.Empty;

            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Productos.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
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

            if (result.Contains("asociada"))
            {
                Product2ToDelete product = new Product2ToDelete();
                product.IsActive = false;
                string salesforceID = string.Empty;

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    salesforceID = db.Productos.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
                }

                if(salesforceID == null)
                {
                    Product2 product2 = new Product2();
                    salesforceID = await _sobjectCRUD.rawQuery(loginResult, product2, DynamicsId, sobject);
                }

                result = await _sobjectCRUD2.updateSobjectByIdAsync(loginResult, product, salesforceID, sobject);

                if (result != "Ok")
                {
                    return result;
                }

                return "Ok";
            }
            
            return result;
        }

        public Product2 getOne(string Id)
        {
            CompanyKey companyKey;
            Context context;

            //Declarando los objetos necesarios para el get de las transferencias
            Item item;
            ItemKey itemKey;
            
            //Inicializando el objeto que voy a enviar a salesforce
            Product2 product = new Product2();
            
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
            itemKey = new ItemKey();
            itemKey.Id = Id;

            item = wsDynamicsGP.GetItemByKey(itemKey, context);          
            
            // Close the service 
            if (wsDynamicsGP.State != CommunicationState.Faulted)
            {
                wsDynamicsGP.Close();
            }

            using (DevelopmentDbContext db = new DevelopmentDbContext())
            {
                //Completando el objeto
                product.Id_External__c = Id;
                product.ProductCode = Id;
                product.Name = item.Description;
                product.Family = item.GenericDescription;
                product.Clases_de_Division_Medica__c = item.UserCategoryList2;
                product.Categoria__c = item.UserCategoryList3;
                product.Origen__c = item.UserCategoryList5;
                product.IsActive = true;
                product.Cost__c = item.CurrentCost.Value;
                product.Description = item.Description;
                switch (item.SalesTaxBasis.Value.ToString().Trim().ToLower())
                {
                    case "taxable": product.Opciones_de_impuestos__c = "Gravable"; break;
                    case "nontaxable": product.Opciones_de_impuestos__c = "No Gravable"; break;
                    case "basedoncustomer": product.Opciones_de_impuestos__c = "Basado en el proveedor"; break;
                }
                
                product.Tipo__c = item.UserCategoryList4;
                string test = db.Database.SqlQuery<string>($"SELECT TOP 1 PropertyValue FROM IV00101Personalizacion WHERE PropertyName = 'Tests/Dosis' AND ITEMNMBR = '{Id}'").FirstOrDefault();

                if(string.IsNullOrEmpty(test))
                {
                    product.Tests_Dosis__c = 0;
                }
                else
                {
                    product.Tests_Dosis__c = Convert.ToDecimal(test.Trim());
                }
                
                return product;
            }
        }
    }
}
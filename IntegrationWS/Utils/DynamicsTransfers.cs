using IntegrationWS.DynamicsGPService;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace IntegrationWS.Utils
{
    public class DynamicsTransfers : IDynamicsTransfers
    {
        public ProductTransfer getOne()
        {
            CompanyKey companyKey;
            Context context;

            //Declarando los objetos necesarios para el get de las transferencias
            InventoryKey inventoryKey;
            InventoryTransfer inventoryTransfer;

            //Declarando la variable de productos para el nombre de los productos
            ItemKey itemKey;
            Item item;

            //Inicializando el objeto que voy a enviar a salesforce
            ProductTransfer transferPartsSf = new ProductTransfer();

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
            inventoryKey = new InventoryKey();
            inventoryKey.Id = "TRA000004";            

            inventoryTransfer = wsDynamicsGP.GetInventoryTransferByKey(inventoryKey, context);            

            //Create an Item key
            itemKey = new ItemKey();
            itemKey.Id = inventoryTransfer.Lines[0].ItemKey.Id;

            item = wsDynamicsGP.GetItemByKey(itemKey, context);            

            // Close the service 
            if (wsDynamicsGP.State != CommunicationState.Faulted)
            {
                wsDynamicsGP.Close();
            }

            //Completando el objeto
            transferPartsSf.QuantitySent = inventoryTransfer.Lines[0].Quantity.Value;
            transferPartsSf.QuantityUnitOfMeasure = "Unidad";
            transferPartsSf.SourceLocationId = inventoryTransfer.Lines[0].WarehouseFromKey.Id;
            transferPartsSf.DestinationLocationId = inventoryTransfer.Lines[0].WarehouseToKey.Id;
            transferPartsSf.Id_External__c = inventoryKey.Id;

            return transferPartsSf;
        }
    }
}
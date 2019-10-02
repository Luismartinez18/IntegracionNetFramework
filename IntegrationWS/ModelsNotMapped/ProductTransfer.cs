using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ProductTransfer
    {
        public string Producto__c { get; set; }
        public decimal QuantitySent { get; set; }
        public string QuantityUnitOfMeasure { get; set; }
        public string SourceLocationId { get; set; }
        public string DestinationLocationId { get; set; }
        public string Id_External__c { get; set; }
    }
}
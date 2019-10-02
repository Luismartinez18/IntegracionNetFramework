using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ProductItemSimple
    {
        public string LocationId { get; set; }
        public string Product2Id { get; set; }
        public decimal QuantityOnHand { get; set; }
        public string QuantityUnitOfMeasure { get; set; }
        public string Id_External__c { get; set; }
    }
}
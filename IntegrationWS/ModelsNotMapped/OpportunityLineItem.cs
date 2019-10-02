using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class OpportunityLineItem
    {
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public string ServiceDate { get; set; }
        public string OpportunityId { get; set; }
        public decimal UnitPrice { get; set; }
        public string Product2Id { get; set; }
        public string Integration_Dynamics_Id__c { get; set; }
    }
}
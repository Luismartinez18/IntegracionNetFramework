using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class OpportunityToUpdate
    {
        public string Description { get; set; } 
        public string StageName { get; set; } 
        public DateTime CloseDate { get; set; }
        public decimal Amount { get; set; } 
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string NextStep { get; set; }
        public decimal Probability { get; set; }
        public string OwnerId { get; set; }  
        public string Type { get; set; }
        public string Oportunidad_Vendedor__c { get; set; }
        public string Referencia__c { get; set; }
        public string Factura_de_origen__c { get; set; }
        public string URL_Factura_de_origen__c { get; set; }
    }
}
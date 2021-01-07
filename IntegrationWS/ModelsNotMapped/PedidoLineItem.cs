using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class PedidoLineItem
    {
        public decimal Quantity__c { get; set; }
        public string Description__c { get; set; }
        public string Pedidos__c { get; set; }
        public decimal UnitPrice__c { get; set; }
        public decimal Subtotal__c { get; set; }
        public decimal Impuesto__c { get; set; }
        public decimal TotalPrice__c { get; set; }
        public string Producto__c { get; set; }
        public DateTime Fecha__c { get; set; }
        public string Integration_Dynamics_Id__c { get; set; }
        
    }
}
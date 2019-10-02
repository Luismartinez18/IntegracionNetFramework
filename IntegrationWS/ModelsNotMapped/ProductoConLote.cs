using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ProductoConLote
    {
        public string Oportunidad__c { get; set; }
        public decimal Cantidad__c { get; set; }
        public string Producto__c { get; set; }
        public decimal Precio_Unitario__c { get; set; }
        public string Lote_o_Serie__c { get; set; }
        public string Id_External__c { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class Detalle_de_Excepci_n_FEFO__c
    {
        public string Lote_ignorado__c { get; set; }
        public string Lote_seleccionado__c { get; set; }
        public string Producto__c { get; set; } 
        public DateTime Vencimiento_ignorado__c { get; set; } 
        public string Excepci_n_FEFO__c { get; set; } 
        public DateTime Vencimiento_seleccionado__c { get; set; }
        public string Id_External__c { get; set; }
    }
}
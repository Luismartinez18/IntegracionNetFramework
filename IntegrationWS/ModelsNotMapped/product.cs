using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class product
    {
        public decimal QuantityConsumed__c { get; set; }
        public string C_digo_del_producto__c { get; set; }
        public string QuantityUnitOfMeasure__c { get; set; }
        public string Locker__c { get; set; }
        public string Lote__c { get; set; }
        public string Serie__c { get; set; }
        public DateTime Fecha_de_vencimiento__c { get; set; }
        public decimal Descuento__c { get; set; }
    }
}
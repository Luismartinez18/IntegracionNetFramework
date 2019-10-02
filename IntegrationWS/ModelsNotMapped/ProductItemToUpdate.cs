using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ProductItemToUpdate
    {
        public decimal QuantityOnHand { get; set; }
        public DateTime? Fecha_de_vencimiento_Lote__c { get; set; }
    }
}
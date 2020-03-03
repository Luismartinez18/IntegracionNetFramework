using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class OpportunityLineItemModel
    {
        public decimal Cantidad { get; set; }
        public string UnidadDeMedida { get; set; } 
        public string CodigoDeProducto { get; set; }
        public decimal Descuento { get; set; } 
    }
}
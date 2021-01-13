using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ContratoLineItem
    {
        public string Producto__c { get; set; }
        public string Contrato_de_servicio__c { get; set; }
        public decimal Cantidad__c { get; set; }
        public decimal Precio__c { get; set; }
        public DateTime Fecha_de_inicio__c { get; set; }
        public DateTime Fecha_de_finalizacion__c { get; set; }
        public DateTime Inicio_de_facturacion__c { get; set; }
        public DateTime Fin_de_facturacion__c { get; set; }
        public decimal Subtotal__c { get; set; }
        public decimal Precio_total__c { get; set; }
        public string Integration_Dynamics_Id__c { get; set; }
    }
}
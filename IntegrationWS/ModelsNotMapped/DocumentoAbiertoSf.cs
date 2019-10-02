using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class DocumentoAbiertoSf
    {
        public string Nombre_de_la_cuenta__c { get; set; }
        public string Terminos_del_cliente__c { get; set; }
        public string Tipo_de_documento__c { get; set; }
        public string Oportunidad__c { get; set; }
        public string Id_External__c { get; set; }
        public DateTime Fecha_del_documento__c { get; set; }
        public DateTime Fecha_de_ltimo_pago__c { get; set; }
        public DateTime Fecha_de_vencimiento__c { get; set; }
        public decimal Monto_del_documento__c { get; set; }
        public decimal Monto_pendiente__c { get; set; }
        public decimal Balance_actual__c { get; set; }
        public decimal X0_a_30_d_as__c { get; set; }
        public decimal X31_a_60_d_as__c { get; set; }
        public decimal X61_a_90_d_as__c { get; set; }
        public decimal X91_a_120_d_as__c { get; set; }
        public decimal X121_a_150_d_as__c { get; set; }
        public decimal X151_d_as_y_mas__c { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class OpportunitySf
    {
        public string Description { get; set; }
        public string StageName { get; set; } 
        public DateTime CloseDate { get; set; } 
        public decimal Amount { get; set; }
        public string Pricebook2Id { get; set; } 
        public string AccountId { get; set; }
        public string Name { get; set; } 
        public string Factura_Dynamics__c { get; set; } 
        public string NextStep { get; set; }
        public decimal Probability { get; set; } 
        public string OwnerId { get; set; }        
        public string Type { get; set; }
        public string Oportunidad_Vendedor__c { get; set; }
        public string CurrencyIsoCode { get; set; }
        public decimal Monto_Dynamics__c { get; set; }
        public string Referencia__c { get; set; }
        public string Factura_de_origen__c { get; set; }
        public string URL_Factura_de_origen__c { get; set; }
        public string Generar_devoluci_n__c { get; set; }
        public string Usuario_que_realiz_pedido_dev__c { get; set; }
        public string Usuario_que_realiz_pedido__c { get; set; }
        public int? Tiempo_en_que_se_libera_de_C_C__c { get; set; }
        public int? Tiempo_de_recogida__c { get; set; }
        public int? Tiempo_de_facturaci_n__c { get; set; }
        public string COMMNTID__c { get; set; }
    }
}
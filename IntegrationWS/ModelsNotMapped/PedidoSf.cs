using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class PedidoSf
    {
        public string Cuenta__c { get; set; } 
        public DateTime Fecha__c { get; set; }
        public string Description__c { get; set; }
        public int? EstaAnulado__c { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string Propietario__c { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Pricebook2Id { get; set; }
        public string Pedido_Dynamics__c { get; set; }
        public string Pedido_Vendedor__c { get; set; }
        public string Vendedor__c { get; set; }
        //public string PoNumber { get; set; }
        public string CurrencyIsoCode { get; set; }
        public string ShippingStreet__c { get; set; }
        public string ShippingCity__c { get; set; }
        public string ShippingState__c { get; set; }
        public string ShippingPostalCode__c { get; set; }
        public string ShippingCountry__c { get; set; }
        public decimal? Subtotal__c { get; set; }
        public decimal? DTO_Comercial__c { get; set; }
        public decimal? Impuesto__c { get; set; }
        public decimal? Total__c { get; set; }
        public decimal? Miscelaneos__c { get; set; }
        public decimal? Flete__c { get; set; }
        public string Comentario__c { get; set; }
        public string Estado__c { get; set; }

    }
}
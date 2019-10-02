using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class AccountSf
    {
        public string Id_External__c { get; set; }
        public string Name { get; set; }
        public string Clase_de_Cliente__c { get; set; }
        public string Razon_Social__c { get; set; }
        public string BillingStreet { get; set; }
        public string BillingCountry { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string ShippingStreet { get; set; }
        public string Sector__c { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingPostalCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Correo_Electronico__c { get; set; }
        public decimal Limite_de_credito__c { get; set; }
        public string Condiciones_de_pago__c { get; set; }
        public decimal Monto_disponible__c { get; set; }
        public int Dias_de_retraso__c { get; set; }
        public string OwnerId { get; set; }
        public string Lista_de_precios__c { get; set; }
        public string RNC__c { get; set; }
        public string Region__c { get; set; }
        public string Cuenta_desactivada__c { get; set; }
        public string Cuenta_inactiva__c { get; set; }
        public string Description { get; set; }
        public string Representante_de_ventas_Diagnostica__c { get; set; }
        public string Representante_de_ventas_Hospitalaria__c { get; set; }
        public string Representante_de_ventas_M_dica__c { get; set; }
    }
}
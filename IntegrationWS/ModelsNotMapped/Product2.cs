using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class Product2
    {
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string Family { get; set; }
        public string Id_External__c { get; set; }
        public string Clases_de_Division_Medica__c { get; set; }
        public string Categoria__c { get; set; }
        public string Origen__c { get; set; }
        public bool IsActive { get; set; }
        public decimal Cost__c { get; set; }
        public string Description { get; set; }
        public string Tipo__c { get; set; }
        public decimal Tests_Dosis__c { get; set; }
        public string Opciones_de_impuestos__c { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class AssetToUpdate
    {
        public string AccountId { get; set; }
        public string Description { get; set; }
        public DateTime? InstallDate { get; set; }
        public string Name { get; set; }
        public string CurrencyIsoCode { get; set; }
        public decimal? Price { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public string Fabricante__c { get; set; }
        public DateTime? UsageEndDate { get; set; }
        public string SerialNumber { get; set; }
        public string Etiqueta_de_activo__c { get; set; }
        public string Clase_de_activo__c { get; set; }
        public string Id_de_Estructura__c { get; set; }
        public int IsInternal { get; set; }
    }
}
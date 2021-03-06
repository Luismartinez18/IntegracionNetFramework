using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace IntegrationWS.ModelsNotMapped
{
    public class PricebookEntry
    {
        public byte IsActive { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Pricebook2Id { get; set; }
        public string Product2Id { get; set; }
        public int UseStandardPrice { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CurrencyIsoCode { get; set; }
    }
}
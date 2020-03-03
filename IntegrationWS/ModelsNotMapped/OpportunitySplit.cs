using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class OpportunitySplit
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? SplitAmount { get; set; }
        public string SplitOwnerId { get; set; }
        public string OpportunityId { get; set; }
        public string SplitNote { get; set; }
        public decimal? SplitPercentage { get; set; }
        public string SplitTypeId { get; set; }
        public decimal? Importe_Dividido__c { get; set; }
    }
}
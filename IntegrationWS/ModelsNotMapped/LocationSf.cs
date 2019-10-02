using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class LocationSf
    {
        public string Name { get; set; }
        public string Id_External__c { get; set; }
        public string Description { get; set; }
        public bool IsInventoryLocation { get; set; }
        public bool IsMobile { get; set; }
    }
}
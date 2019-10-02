using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class PricebookEntryToUpdate
    {
        public byte IsActive { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
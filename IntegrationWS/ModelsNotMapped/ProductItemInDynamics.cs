using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ProductItemInDynamics
    {
        public string ITEMNMBR { get; set; }
        public decimal QTYONHND { get; set; }
        public string LOCNCODE { get; set; }
        public string BASEUOFM { get; set; }
        public DateTime MFGDATE { get; set; }
        public DateTime EXPNDATE { get; set; }
    }
}
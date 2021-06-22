using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class Price
    {
        public string Customer { get; set; }
        public string Product { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public string Division { get; set; }
        public decimal ExistenciaToDisplay { get; set; }
    }
}
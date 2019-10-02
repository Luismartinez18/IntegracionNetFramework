using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string DynamicsId { get; set; }
        public string SalesforceId { get; set; }
    }
}
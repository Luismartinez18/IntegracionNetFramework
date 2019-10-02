using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class BulkApi
    {
        public string @object { get; set; }
        public string contentType { get; set; }
        public string operation { get; set; }
        public string lineEnding { get; set; }
    }
}
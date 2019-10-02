using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class SfResponseModel
    {
        public string authToken { get; set; }
        public string serviceURL { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string version { get; set; }
    }
}
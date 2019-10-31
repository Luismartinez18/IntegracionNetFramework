using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Utils
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Description { get; set; }
        public string ReasonPhrase { get; set; }
    }
}
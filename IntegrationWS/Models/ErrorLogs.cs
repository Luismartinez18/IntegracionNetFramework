using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class ErrorLogs
    {
        public int Id { get; set; }
        public DateTime FechaDelError { get; set; }
        public string Error { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class EquiposDeCuenta
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AccountId { get; set; }
        public string TeamMemberRole { get; set; }
        public string SalesforceId { get; set; }
    }
}
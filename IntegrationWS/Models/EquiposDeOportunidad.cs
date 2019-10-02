using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class EquiposDeOportunidad
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string TeamMemberRole { get; set; }

        [StringLength(100)]
        public string OpportunityId { get; set; }

        [StringLength(100)]
        public string UserId { get; set; }

        [StringLength(100)]
        public string SalesforceId { get; set; }
    }
}
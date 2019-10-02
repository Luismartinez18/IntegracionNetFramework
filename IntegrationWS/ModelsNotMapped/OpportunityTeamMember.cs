using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class OpportunityTeamMember
    {
        public string OpportunityAccessLevel { get; set; }
        public string TeamMemberRole { get; set; }
        public string OpportunityId { get; set; }
        public string UserId { get; set; }
    }
}
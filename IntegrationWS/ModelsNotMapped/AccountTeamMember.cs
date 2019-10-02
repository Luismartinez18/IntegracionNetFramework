using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class AccountTeamMember
    {
        public string AccountAccessLevel { get; set; }
        public string AccountId { get; set; }
        public string CaseAccessLevel { get; set; }
        public string ContactAccessLevel { get; set; }
        public string OpportunityAccessLevel { get; set; }
        public string TeamMemberRole { get; set; }
        public string UserId { get; set; }
            
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class JournalEntryHeader
    {
        public int CompanyCode { get; set; }
        public int JournalId { get; set; }
        public DateTime? DocDate { get; set; }
        public string BatchKey { get; set; }
        public string Payroll { get; set; }
        public string Currency { get; set; }
        public List<JournalEntryDetail> Detail = new List<JournalEntryDetail>();
        public string InterId { get; set; }
    }
    public class JournalEntryDetail 
    {
        public string Account { get; set; }
        public int SQNCLINE { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? DebitAmount { get; set; }
    }
}
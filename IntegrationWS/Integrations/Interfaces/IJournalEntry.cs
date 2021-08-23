using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Integrations.Interfaces
{
    public interface IJournalEntry
    {
        Task<string> SendDynamics(JournalEntryHeader add);
        Task<bool> ExistsJournalEntry(int JournalId);
        Task<bool> ExistsPayroll(string Reference,string InterId);
        Task<bool> ExistsAccount(string Account, string InterId);
        Task<int> GetNumJournalEntry(string InterId);
    }
}

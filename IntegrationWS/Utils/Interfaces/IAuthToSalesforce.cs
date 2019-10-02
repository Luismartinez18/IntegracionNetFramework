using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Utils.Interfaces
{
    public interface IAuthToSalesforce
    {
        Task<string> Login();
        Task<string> Logout(string token, string serviceURL);
    }
}

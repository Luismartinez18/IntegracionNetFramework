using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Integrations.Interfaces
{
    public interface IActivos
    {
        Asset1 getOne(string Id);
        Task<string> create(string Id, string loginResult, string authToken, string serviceURL);
        Task<string> update(string Id, string loginResult, string authToken, string serviceURL);
        Task<string> delete(string Id, string loginResult, string DynamicsId);
    }
}

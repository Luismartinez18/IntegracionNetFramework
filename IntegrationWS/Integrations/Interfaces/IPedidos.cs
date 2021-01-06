using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Integrations.Interfaces
{
    public interface IPedidos
    {
        Task<PedidoSf> getOne(string Id, string loginResult);
        Task<string> create(string Id, string loginResult, string authToken, string serviceURL);
        Task<string> update(string Id, string loginResult, string authToken, string serviceURL, string SalesforceId);
        Task<string> delete(string loginResult, string Id);
    }
}

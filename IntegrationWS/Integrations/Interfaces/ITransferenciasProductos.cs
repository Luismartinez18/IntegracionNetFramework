using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Integrations.Interfaces
{
    public interface ITransferenciasProductos
    {
        Task<string> create(string Id, string loginResult, string authToken, string serviceURL);
        Task<string> update(string Id, string loginResult, string authToken, string serviceURL);
        ProductTransfer getOne(string Id);
    }
}

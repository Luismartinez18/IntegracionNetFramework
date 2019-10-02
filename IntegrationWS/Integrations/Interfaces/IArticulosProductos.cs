using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Integrations.Interfaces
{
    public interface IArticulosProductos
    {
        ProductItem getOne(string Id);
        ProductItem getOne200(string Id);
        ProductItem getOne300(string Id);
        Task<string> create(string Id, string loginResult, string authToken, string serviceURL, string table);
        Task<string> update(string Id, string loginResult, string authToken, string serviceURL, string table);
        Task<string> delete(string Id, string loginResult);
    }
}

using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Integrations.Interfaces
{
    public interface IExcepcionesFEFODetalle
    {
        List<Detalle_de_Excepci_n_FEFO__c> getAll(string Id);
        Task<string> create(string Id, string loginResult, string authToken, string serviceURL, string SalesforceId);
        Task<string> delete(string loginResult, string DynamicsId);
    }
}

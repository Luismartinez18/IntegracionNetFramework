using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class OpportunitiesDTO
    {
        public string Id { get; set; }
        public string CodigoCliente { get; set; }
        public string Name { get; set; }
        public string Sucursal { get; set; }
        public string Vendedor { get; set; }
        public List<OpportunityLineItemModel> OpportunityLineItemModels { get; set; }
    }
}
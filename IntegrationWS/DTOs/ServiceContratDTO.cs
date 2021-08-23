using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class ServiceContratDTO
    {
        public int companyKeyId { get; set; }
        public string NombreDeLaCuenta { get; set; }
        public string NumeroDeLaCuenta { get; set; }
        public string ServiceContratNumber { get; set; }
        public decimal Totaldehorastrabajadas { get; set; }
        public string Comentarios { get; set; }
        public decimal descuento { get; set; }
        public List<ServiceContratDetalleDTO> products { get; set; }
    }
}
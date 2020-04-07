using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class PresupuestoDTO
    {
        public string Comentarios { get; set; }
        public string QuoteNumber { get; set; }
        public string NombreDeLaCuenta { get; set; }
        public string NumeroDeLaCuenta { get; set; }
        public int companyKeyId { get; set; }
        public decimal descuento { get; set; }
        public string usuario { get; set; }
        public List<presupuesto_producto> presupuesto_producto { get; set; }
    }
}
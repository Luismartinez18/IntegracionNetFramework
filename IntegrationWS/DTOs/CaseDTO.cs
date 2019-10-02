using IntegrationWS.ModelsNotMapped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class CaseDTO
    {
        public decimal Totaldehorastrabajadas { get; set; }
        public string Subject { get; set; }
        public List<product> products { get; set; }
        public string Nmerodeserie { get; set; }
        public bool Facturaralcliente { get; set; }
        public bool Facturainterna { get; set; }
        public string Comentarios { get; set; }
        public string CaseNumber { get; set; }
        public List<ordenConCita> OrdenesConCitas { get; set; }
        public string NombreDeLaCuenta { get; set; }
        public string NumeroDeLaCuenta { get; set; }
        public int companyKeyId { get; set; }
        public List<CargoAdicional> CargosAdicionales { get; set; }
        public decimal descuento { get; set; }
    }      
}
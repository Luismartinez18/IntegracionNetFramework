using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class CargoAdicional
    {
        public string Codigo { get; set; }
        public decimal Cantidad { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
    }
}
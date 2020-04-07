using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class presupuesto_producto
    {
        public string Codigo { get; set; }
        public decimal Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string NombreDeProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
    }
}
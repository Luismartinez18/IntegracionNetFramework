using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class Notificacion_de_pedido_espera
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string SalesforceId { get; set; }

        [StringLength(100)]
        public string DynamicsId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class RegistroGastoDTO
    {
        
        [StringLength(maximumLength: 13, MinimumLength = 9, ErrorMessage = "El RNC debe tener 9 u 11 caracteres.")]
        public string RNC__c { get; set; }        
        [StringLength(maximumLength: 11, MinimumLength = 11, ErrorMessage = "El NCF debe tener 11 caracteres.")]
        public string NCF__c { get; set; }
        public string Numero_de_factura__c { get; set; }
        [Required]
        public DateTime Fecha_de_consumo__c { get; set; }
        [Required]
        public decimal Monto__c { get; set; }
        [Required]
        public string Departamento__c { get; set; }

        [Required]
        public string Tipo_de_gasto_NFC__c { get; set; }

        public bool Propina_legal__c { get; set; }
        public string Divisi_n__c { get; set; }
    }
}
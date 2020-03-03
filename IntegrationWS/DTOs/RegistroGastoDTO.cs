using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class RegistroGastoDTO
    {
        [Required]
        public string Name { get; set; }
        public string RNC__c { get; set; }          
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
        public bool Exento_de_ITBIS__c { get; set; }
        public string Divisi_n__c { get; set; }
        [Required]
        public string Descripci_n__c { get; set; }
    }
}